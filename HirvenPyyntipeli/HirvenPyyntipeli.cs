using System;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author tuomas
/// @version 03.03.2026
/// <summary>
/// Hirven Pyyntipeli - harjoitustyö.
/// Vaihe 2: runko + UI + tähtäin + ampuminen + hirven spawnaus + katoaminen + metsätausta + ase.
/// </summary>
public class HirvenPyyntipeli : Game
{
    private const double KentanLeveys = 1000;
    private const double KentanKorkeus = 700;

    // Layerit (pienempi = taaempi)
    // HUOM: ÄLÄ mene liian pieniin (esim. -4), Jypeli voi kaatua
    private const int LayerTaustaBack = -3;
    private const int LayerTaustaLights = -2; // säteet PUUTEN taakse (middle-puiden takana)
    private const int LayerTaustaMiddle = -1;

    private const int LayerPeli = 0;
    private const int LayerForeground = 1;

    private IntMeter pisteet;
    private Label pisteNaytto;

    // Tähtäin UI-overlaynä
    private Widget tahtain;

    // Ase UI:na ruudun alareunassa
    private Widget ase;

    private GameObject hirvi;

    // Spawnausasetukset
    private Timer spawnAjastin;

    private const double SpawnVali = 3.0;
    private const double HirvenElinaika = 3.0;
    private const double ViiveOsumanJalkeen = 2.0;

    private bool hirviElossa;
    private bool spawnCooldown;

    // Taustakerrokset
    private GameObject taustaBack;
    private GameObject taustaMiddle;
    private GameObject taustaLights;
    private GameObject taustaFront;

    public override void Begin()
    {
        LuoKentta();
        LuoMetsaTausta();

        LuoKayttoliittyma();
        LuoTahtain();
        LuoAse();
        AsetaOhjaimet();

        KaynnistaHirviSpawni();
        AloitaPeli();
    }

    private void LuoKentta()
    {
        SetWindowSize((int)KentanLeveys, (int)KentanKorkeus);

        Level.Size = new Vector(KentanLeveys, KentanKorkeus);
        Camera.ZoomToLevel();

        Level.Background.Color = Color.ForestGreen; // varaväri
    }

    // ====== TAUSTA (4 kerrosta) ======

    private void LuoMetsaTausta()
    {
        // Järjestys: back -> lights -> middle -> (peli) -> front
        taustaBack = LuoTaustaKerros("parallax-forest-back-trees", LayerTaustaBack);
        taustaLights = LuoTaustaKerros("parallax-forest-lights", LayerTaustaLights);
        taustaMiddle = LuoTaustaKerros("parallax-forest-middle-trees", LayerTaustaMiddle);

        // Front-trees: jos tämä peittää hirven liikaa, vaihda layeriksi LayerPeli - 1
        taustaFront = LuoTaustaKerros("parallax-forest-front-trees", LayerForeground);
    }

    private GameObject LuoTaustaKerros(string kuvanNimi, int layer)
    {
        Image img = LoadImage(kuvanNimi);
        img.Scaling = ImageScaling.Nearest;

        GameObject kerros = new GameObject(Level.Width, Level.Height);
        kerros.Image = img;
        kerros.Position = Level.Center;

        Add(kerros, layer);
        return kerros;
    }

    // ====== UI ======

    private void LuoKayttoliittyma()
    {
        pisteet = new IntMeter(0);

        pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 120;
        pisteNaytto.Y = Screen.Top - 50;
        pisteNaytto.TextColor = Color.White;
        pisteNaytto.BorderColor = Color.Transparent;
        pisteNaytto.Color = new Color(0, 0, 0, 80);

        PaivitaPisteTeksti();
        pisteet.Changed += delegate { PaivitaPisteTeksti(); };

        Add(pisteNaytto);
    }

    private void PaivitaPisteTeksti()
    {
        pisteNaytto.Text = "Pisteet: " + pisteet.Value;
    }

    private void LuoTahtain()
    {
        tahtain = new Widget(18, 18, Shape.Circle);
        tahtain.Color = Color.Red;

        Add(tahtain);

        Mouse.IsCursorVisible = false;
        Mouse.ListenMovement(0.0, PaivitaTahtain, null);
        PaivitaTahtain();
    }

    private void PaivitaTahtain()
    {
        tahtain.Position = Mouse.PositionOnScreen;
    }

    private void LuoAse()
    {
        ase = new Widget(220, 50, Shape.Rectangle);
        ase.Color = new Color(60, 45, 30);
        ase.BorderColor = Color.Transparent;

        Widget piippu = new Widget(140, 14, Shape.Rectangle);
        piippu.Color = new Color(40, 40, 40);
        piippu.BorderColor = Color.Transparent;

        Widget tahti = new Widget(10, 10, Shape.Circle);
        tahti.Color = new Color(30, 30, 30);
        tahti.BorderColor = Color.Transparent;

        piippu.X = ase.Width / 2 - piippu.Width / 2;
        piippu.Y = 6;

        tahti.X = ase.Width / 2 - 25;
        tahti.Y = 16;

        ase.Add(piippu);
        ase.Add(tahti);

        Add(ase);

        ase.X = 0;
        ase.Y = Screen.Bottom + 45;
    }

    private void AsetaOhjaimet()
    {
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Ammu, "Ammu");
    }

    private void AloitaPeli()
    {
        MessageDisplay.Add("Peli käynnissä!");
    }

    // ====== HIRVEN SPAWNAUS ======

    private void KaynnistaHirviSpawni()
    {
        spawnAjastin = new Timer();
        spawnAjastin.Interval = SpawnVali;
        spawnAjastin.Timeout += SpawnHirvi;
        spawnAjastin.Start();

        SpawnHirvi();
    }

    private void SpawnHirvi()
    {
        if (spawnCooldown) return;
        if (hirviElossa) return;

        hirvi = new GameObject(80, 60, Shape.Rectangle);
        hirvi.Color = Color.Brown;
        hirvi.Position = ArvoSatunnainenSijainti(hirvi.Width, hirvi.Height);

        Add(hirvi, LayerPeli);

        hirviElossa = true;

        Timer.SingleShot(HirvenElinaika, delegate
        {
            if (hirviElossa && hirvi != null)
            {
                hirvi.Destroy();
                hirvi = null;
                hirviElossa = false;
            }
        });
    }

    private void AsetaSeuraavaHirviViiveella(double viive)
    {
        spawnCooldown = true;

        Timer.SingleShot(viive, delegate
        {
            spawnCooldown = false;
            hirviElossa = false;
            SpawnHirvi();
        });
    }

    private Vector ArvoSatunnainenSijainti(double olionLeveys, double olionKorkeus)
    {
        double minX = Level.Left + olionLeveys / 2.0;
        double maxX = Level.Right - olionLeveys / 2.0;

        double minY = Level.Bottom + olionKorkeus / 2.0 + 80;
        double maxY = Level.Top - olionKorkeus / 2.0 - 60;

        double x = RandomGen.NextDouble(minX, maxX);
        double y = RandomGen.NextDouble(minY, maxY);

        return new Vector(x, y);
    }

    private bool KlikattiinkoHirvea()
    {
        if (!hirviElossa || hirvi == null) return false;

        Vector p = Mouse.PositionOnWorld;
        return (p.X >= hirvi.Left && p.X <= hirvi.Right &&
                p.Y >= hirvi.Bottom && p.Y <= hirvi.Top);
    }

    private void Ammu()
    {
        if (KlikattiinkoHirvea())
        {
            pisteet.Value += 10;

            if (hirvi != null) hirvi.Destroy();
            hirvi = null;
            hirviElossa = false;

            MessageDisplay.Add("Osuma!");

            AsetaSeuraavaHirviViiveella(ViiveOsumanJalkeen);
        }
        else
        {
            MessageDisplay.Add("Huti");
        }
    }
}