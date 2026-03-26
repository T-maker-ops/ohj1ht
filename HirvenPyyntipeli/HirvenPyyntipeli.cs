using System;
using System.Collections.Generic;
using Jypeli;


/// @author tuomas
/// @version 26.03.2026
/// <summary>
/// Hirven Pyyntipeli - harjoitustyö.
/// Pelaaja ampuu hirviä ja välttelee viattomia eläimiä.
/// </summary>
public class HirvenPyyntipeli : Game
{
    /// <summary>
    /// Pelikentän leveys.
    /// </summary>
    private const double KentanLeveys = 1000;


    /// <summary>
    /// Pelikentän korkeus.
    /// </summary>
    private const double KentanKorkeus = 700;


    /// <summary>
    /// Taustimmaisen metsän layer.
    /// </summary>
    private const int LayerTaustaBack = -3;


    /// <summary>
    /// Valosäteiden layer.
    /// </summary>
    private const int LayerTaustaLights = -2;


    /// <summary>
    /// Keskimmäisen taustakerroksen layer.
    /// </summary>
    private const int LayerTaustaMiddle = -1;


    /// <summary>
    /// Etummaisen taustakerroksen layer.
    /// </summary>
    private const int LayerForeground = 0;


    /// <summary>
    /// Pelissä liikkuvien eläinten layer.
    /// </summary>
    private const int LayerPeli = 1;


    /// <summary>
    /// Eläinten spawnauksen aikaväli sekunteina.
    /// </summary>
    private const double SpawnVali = 0.5;


    /// <summary>
    /// Eläimen elinaika ruudulla sekunteina.
    /// </summary>
    private const double ElaimenElinaika = 2.8;


    /// <summary>
    /// Pelin kokonaiskesto sekunteina.
    /// </summary>
    private const double Peliaika = 60.0;


    /// <summary>
    /// Peliajan ajastimen päivitysväli sekunteina.
    /// </summary>
    private const double PeliAjastimenVali = 0.1;


    /// <summary>
    /// Suurin sallittu väärien osumien määrä.
    /// </summary>
    private const int MaksimiVirheet = 3;


    /// <summary>
    /// Hirveen osumisesta saatavat pisteet.
    /// </summary>
    private const int HirviPisteet = 10;


    /// <summary>
    /// Väärään eläimeen osumisesta vähennettävät pisteet.
    /// </summary>
    private const int VaaraElainSakko = 5;


    /// <summary>
    /// Eläinten enimmäismäärä ruudulla samaan aikaan.
    /// </summary>
    private const int MaksimiElaimiaRuudulla = 4;


    /// <summary>
    /// Vapaata spawn-sijaintia etsivien yritysten määrä.
    /// </summary>
    private const int SpawnYritystenMaara = 20;


    /// <summary>
    /// Tähtäimen leveys.
    /// </summary>
    private const double TahtaimenLeveys = 18;


    /// <summary>
    /// Tähtäimen korkeus.
    /// </summary>
    private const double TahtaimenKorkeus = 18;


    /// <summary>
    /// Aseen leveys.
    /// </summary>
    private const double AseenLeveys = 280;


    /// <summary>
    /// Aseen korkeus.
    /// </summary>
    private const double AseenKorkeus = 90;


    /// <summary>
    /// Aseen x-sijainti ruudulla.
    /// </summary>
    private const double AseenSijaintiX = 80;


    /// <summary>
    /// Aseen etäisyys ruudun alareunasta.
    /// </summary>
    private const double AseenEtaysAlareunasta = 50;


    /// <summary>
    /// Aseen kulmaan lisättävä korjaus asteina.
    /// </summary>
    private const double AseenKulmaKorjaus = 197;


    /// <summary>
    /// Pistelaskurin x-etäisyys ruudun vasemmasta reunasta.
    /// </summary>
    private const double PisteNaytonEtaisyysVasemmalta = 120;


    /// <summary>
    /// Pistelaskurin y-etäisyys ruudun yläreunasta.
    /// </summary>
    private const double PisteNaytonEtaisyysYlhaalta = 50;


    /// <summary>
    /// Virhenäytön x-etäisyys ruudun vasemmasta reunasta.
    /// </summary>
    private const double VirheNaytonEtaisyysVasemmalta = 120;


    /// <summary>
    /// Virhenäytön y-etäisyys ruudun yläreunasta.
    /// </summary>
    private const double VirheNaytonEtaisyysYlhaalta = 90;


    /// <summary>
    /// Aikanäytön x-etäisyys ruudun oikeasta reunasta.
    /// </summary>
    private const double AikaNaytonEtaisyysOikealta = 120;


    /// <summary>
    /// Aikanäytön y-etäisyys ruudun yläreunasta.
    /// </summary>
    private const double AikaNaytonEtaisyysYlhaalta = 50;


    /// <summary>
    /// Spawnattavan eläimen vähimmäisetäisyys ruudun alareunasta.
    /// </summary>
    private const double ElaimenAlaMarginaali = 80;


    /// <summary>
    /// Aloitusikkunan leveys.
    /// </summary>
    private const double AloitusIkkunanLeveys = 520;


    /// <summary>
    /// Aloitusikkunan korkeus.
    /// </summary>
    private const double AloitusIkkunanKorkeus = 260;


    /// <summary>
    /// Käyttöliittymälabelin taustaväri.
    /// </summary>
    private static readonly Color NayttoTaustaVari = new Color(0, 0, 0, 80);


    /// <summary>
    /// Pelin pistemittari.
    /// </summary>
    private IntMeter pisteet;


    /// <summary>
    /// Pelin virhemittari.
    /// </summary>
    private IntMeter virheet;


    /// <summary>
    /// Pisteiden näyttämiseen käytettävä label.
    /// </summary>
    private Label pisteNaytto;


    /// <summary>
    /// Virheiden näyttämiseen käytettävä label.
    /// </summary>
    private Label virheNaytto;


    /// <summary>
    /// Jäljellä olevan ajan näyttämiseen käytettävä label.
    /// </summary>
    private Label aikaNaytto;


    /// <summary>
    /// Pelaajan tähtäin.
    /// </summary>
    private Widget tahtain;


    /// <summary>
    /// Pelaajan ase.
    /// </summary>
    private Widget ase;


    /// <summary>
    /// Peliaikaa päivittävä ajastin.
    /// </summary>
    private Timer peliAjastin;


    /// <summary>
    /// Eläinten spawnauksesta vastaava ajastin.
    /// </summary>
    private Timer spawnAjastin;


    /// <summary>
    /// Jäljellä oleva peliaika sekunteina.
    /// </summary>
    private double aikaaJaljella;


    /// <summary>
    /// Kertoo, onko peli päättynyt.
    /// </summary>
    private bool peliPaattynyt;


    /// <summary>
    /// Kertoo, onko varsinainen peli käynnissä.
    /// </summary>
    private bool peliKaynnissa;


    /// <summary>
    /// Aseen laukausääni.
    /// </summary>
    private SoundEffect laukausAani;


    /// <summary>
    /// Hirveen osumisesta soitettava ääni.
    /// </summary>
    private SoundEffect hirviAani;


    /// <summary>
    /// Aseen kuva.
    /// </summary>
    private Image aseenKuva;


    /// <summary>
    /// Taulukko pelissä käytettävien eläinten nimistä.
    /// </summary>
    private readonly string[] elaimet = ["hirvi", "janis", "orava"];


    /// <summary>
    /// Lista ruudulla tällä hetkellä olevista eläimistä.
    /// </summary>
    private readonly List<GameObject> aktiivisetElaimet = [];


    /// <summary>
    /// Sanakirja, joka yhdistää eläinolion sen tyyppiin.
    /// </summary>
    private readonly Dictionary<GameObject, string> elainTyypit = [];


    /// <summary>
    /// Sanakirja, joka yhdistää eläimen nimen sen kuvaan.
    /// </summary>
    private readonly Dictionary<string, Image> elainKuvat = [];


    /// <summary>
    /// Sanakirja, joka yhdistää eläimen nimen sen kokoon.
    /// </summary>
    private readonly Dictionary<string, Vector> elainKoot = new()
    {
        { "hirvi", new Vector(160, 260) },
        { "janis", new Vector(90, 110) },
        { "orava", new Vector(80, 100) }
    };


    /// <summary>
    /// Alustaa pelin.
    /// </summary>
    public override void Begin()
    {
        LuoKentta();
        LataaResurssit();
        LuoMetsaTausta();
        LuoKayttoliittyma();
        LuoTahtain();
        LuoAse();
        AsetaOhjaimet();
        NaytaAloitusIkkuna();
    }


    /// <summary>
    /// Luo pelikentän koon ja taustavärin.
    /// </summary>
    private void LuoKentta()
    {
        SetWindowSize((int)KentanLeveys, (int)KentanKorkeus);
        Level.Size = new Vector(KentanLeveys, KentanKorkeus);
        Camera.ZoomToLevel();
        Level.Background.Color = Color.ForestGreen;
    }


    /// <summary>
    /// Lataa pelissä käytettävät kuvat ja äänet kerran pelin alussa.
    /// </summary>
    private void LataaResurssit()
    {
        aseenKuva = LoadImage("kivaari");

        foreach (string elain in elaimet)
        {
            elainKuvat[elain] = LoadImage(elain);
        }

        try
        {
            laukausAani = LoadSoundEffect("aseaani.wav");
        }
        catch (Exception)
        {
            laukausAani = null;
            MessageDisplay.Add("Aseääntä ei löytynyt.");
        }

        try
        {
            hirviAani = LoadSoundEffect("Hirviaani.wav");
        }
        catch (Exception)
        {
            hirviAani = null;
            MessageDisplay.Add("Hirviääntä ei löytynyt.");
        }
    }


    /// <summary>
    /// Luo metsäteemaisen taustan useasta kerroksesta.
    /// </summary>
    private void LuoMetsaTausta()
    {
        LuoTaustaKerros("parallax-forest-back-trees", LayerTaustaBack);
        LuoTaustaKerros("parallax-forest-lights", LayerTaustaLights);
        LuoTaustaKerros("parallax-forest-middle-trees", LayerTaustaMiddle);
        LuoTaustaKerros("parallax-forest-front-trees", LayerForeground);
    }


    /// <summary>
    /// Luo yhden taustakerroksen annetuista tiedoista.
    /// </summary>
    /// <param name="kuvanNimi">Ladattavan kuvan nimi.</param>
    /// <param name="layer">Layer, jolle taustakerros lisätään.</param>
    private void LuoTaustaKerros(string kuvanNimi, int layer)
    {
        Image kuva = LoadImage(kuvanNimi);
        kuva.Scaling = ImageScaling.Nearest;

        GameObject kerros = new(Level.Width, Level.Height)
        {
            Image = kuva,
            Position = Level.Center
        };

        Add(kerros, layer);
    }


    /// <summary>
    /// Luo pelin käyttöliittymän.
    /// </summary>
    private void LuoKayttoliittyma()
    {
        pisteet = new IntMeter(0);
        virheet = new IntMeter(0);
        aikaaJaljella = Peliaika;

        pisteNaytto = new Label
        {
            X = Screen.Left + PisteNaytonEtaisyysVasemmalta,
            Y = Screen.Top - PisteNaytonEtaisyysYlhaalta,
            TextColor = Color.White,
            BorderColor = Color.Transparent,
            Color = NayttoTaustaVari
        };

        virheNaytto = new Label
        {
            X = Screen.Left + VirheNaytonEtaisyysVasemmalta,
            Y = Screen.Top - VirheNaytonEtaisyysYlhaalta,
            TextColor = Color.White,
            BorderColor = Color.Transparent,
            Color = NayttoTaustaVari
        };

        aikaNaytto = new Label
        {
            X = Screen.Right - AikaNaytonEtaisyysOikealta,
            Y = Screen.Top - AikaNaytonEtaisyysYlhaalta,
            TextColor = Color.White,
            BorderColor = Color.Transparent,
            Color = NayttoTaustaVari
        };

        pisteet.Changed += delegate { PaivitaPisteTeksti(); };
        virheet.Changed += delegate { PaivitaVirheTeksti(); };

        PaivitaPisteTeksti();
        PaivitaVirheTeksti();
        PaivitaAikaTeksti();

        Add(pisteNaytto);
        Add(virheNaytto);
        Add(aikaNaytto);
    }


    /// <summary>
    /// Päivittää pistenäytön tekstin.
    /// </summary>
    private void PaivitaPisteTeksti()
    {
        pisteNaytto.Text = "Pisteet: " + pisteet.Value;
    }


    /// <summary>
    /// Päivittää virhenäytön tekstin.
    /// </summary>
    private void PaivitaVirheTeksti()
    {
        virheNaytto.Text = "Virheet: " + virheet.Value + "/" + MaksimiVirheet;
    }


    /// <summary>
    /// Päivittää aikanäytön tekstin.
    /// </summary>
    private void PaivitaAikaTeksti()
    {
        aikaNaytto.Text = "Aika: " + Math.Ceiling(aikaaJaljella);
    }


    /// <summary>
    /// Luo pelaajan tähtäimen.
    /// </summary>
    private void LuoTahtain()
    {
        tahtain = new Widget(TahtaimenLeveys, TahtaimenKorkeus, Shape.Circle)
        {
            Color = Color.Red
        };

        Add(tahtain);

        Mouse.IsCursorVisible = false;
        Mouse.ListenMovement(0.0, PaivitaTahtain, null);
        PaivitaTahtain();
    }


    /// <summary>
    /// Päivittää tähtäimen sijainnin hiiren kohdalle.
    /// </summary>
    private void PaivitaTahtain()
    {
        tahtain.Position = Mouse.PositionOnScreen;
        PaivitaAseenSuunta();
    }


    /// <summary>
    /// Luo pelaajan aseen ruudun alareunaan.
    /// </summary>
    private void LuoAse()
    {
        ase = new Widget(AseenLeveys, AseenKorkeus)
        {
            Image = aseenKuva,
            Color = Color.Transparent,
            BorderColor = Color.Transparent,
            X = AseenSijaintiX,
            Y = Screen.Bottom + AseenEtaysAlareunasta
        };

        ase.MirrorImage();
        Add(ase);
    }


    /// <summary>
    /// Kääntää aseen kohti tähtäintä.
    /// </summary>
    private void PaivitaAseenSuunta()
    {
        if (ase == null) return;

        Vector suunta = Mouse.PositionOnScreen - ase.Position;
        ase.Angle = suunta.Angle + Angle.FromDegrees(AseenKulmaKorjaus);
    }


    /// <summary>
    /// Asettaa pelin ohjaimet.
    /// </summary>
    private void AsetaOhjaimet()
    {
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Ammu, "Ammu");
    }


    /// <summary>
    /// Näyttää pelin aloitusikkunan.
    /// </summary>
    private void NaytaAloitusIkkuna()
    {
        Window aloitusIkkuna = new Window(AloitusIkkunanLeveys, AloitusIkkunanKorkeus)
        {
            Color = new Color(30, 30, 30, 220)
        };

        Label otsikko = new Label
        {
            Text = "Hirvenpyyntipeli!",
            TextColor = Color.White,
            X = 0,
            Y = 70
        };

        Label ohjeTeksti = new Label
        {
            Text = "Ammu niin monta hirveä kuin ehdit minuutin aikana!\n" +
                   "Varo kuitenkin metsän muita eläimiä!\n\n" +
                   "Oletko valmis metsästämään?",
            TextColor = Color.White,
            X = 0,
            Y = 10
        };

        PushButton aloitaNappi = new PushButton("Aloita peli")
        {
            X = 0,
            Y = -80
        };

        aloitaNappi.Clicked += delegate
        {
            aloitusIkkuna.Destroy();
            AloitaPeli();
        };

        aloitusIkkuna.Add(otsikko);
        aloitusIkkuna.Add(ohjeTeksti);
        aloitusIkkuna.Add(aloitaNappi);

        Add(aloitusIkkuna);
    }


    /// <summary>
    /// Käynnistää varsinaisen pelin.
    /// </summary>
    private void AloitaPeli()
    {
        if (peliKaynnissa) return;

        peliKaynnissa = true;
        MessageDisplay.Add("Ammu vain hirviä!");
        KaynnistaPeliAjastin();
        SpawnElain();
        KaynnistaSpawnAjastin();
    }


    /// <summary>
    /// Käynnistää peliaikaa vähentävän ajastimen.
    /// </summary>
    private void KaynnistaPeliAjastin()
    {
        peliAjastin = new Timer
        {
            Interval = PeliAjastimenVali
        };

        peliAjastin.Timeout += PaivitaPeliaikaa;
        peliAjastin.Start();
    }


    /// <summary>
    /// Käynnistää eläinten spawnauksesta vastaavan ajastimen.
    /// </summary>
    private void KaynnistaSpawnAjastin()
    {
        spawnAjastin = new Timer
        {
            Interval = SpawnVali
        };

        spawnAjastin.Timeout += SpawnElain;
        spawnAjastin.Start();
    }


    /// <summary>
    /// Päivittää jäljellä olevaa peliaikaa ja päättää pelin ajan loppuessa.
    /// </summary>
    private void PaivitaPeliaikaa()
    {
        if (peliPaattynyt) return;
        if (!peliKaynnissa) return;

        aikaaJaljella -= PeliAjastimenVali;

        if (aikaaJaljella < 0)
        {
            aikaaJaljella = 0;
        }

        PaivitaAikaTeksti();

        if (aikaaJaljella <= 0)
        {
            PaataPeliAjanLoppuessa();
        }
    }


    /// <summary>
    /// Luo uuden eläimen ruudulle, jos peli on käynnissä ja tilaa on vapaana.
    /// </summary>
    private void SpawnElain()
    {
        if (peliPaattynyt) return;
        if (!peliKaynnissa) return;
        if (aktiivisetElaimet.Count >= MaksimiElaimiaRuudulla) return;

        string elaimenNimi = ArvoSatunnainenElain();
        Vector koko = HaeElaimenKoko(elaimenNimi);

        GameObject elain = new(koko.X, koko.Y)
        {
            Image = elainKuvat[elaimenNimi],
            Position = ArvoVapaaSijainti(koko.X, koko.Y)
        };

        Add(elain, LayerPeli);

        aktiivisetElaimet.Add(elain);
        elainTyypit[elain] = elaimenNimi;

        Timer.SingleShot(ElaimenElinaika, delegate
        {
            if (peliPaattynyt) return;
            if (!elainTyypit.ContainsKey(elain)) return;

            PoistaElain(elain);
        });
    }


    /// <summary>
    /// Arpoo yhden eläimen taulukosta.
    /// </summary>
    /// <returns>Arvotun eläimen nimi.</returns>
    private string ArvoSatunnainenElain()
    {
        int indeksi = RandomGen.NextInt(0, elaimet.Length);
        return elaimet[indeksi];
    }


    /// <summary>
    /// Palauttaa annetun eläimen koon.
    /// </summary>
    /// <param name="elaimenNimi">Eläimen nimi.</param>
    /// <returns>Eläimen koko vektorina.</returns>
    private Vector HaeElaimenKoko(string elaimenNimi)
    {
        return elainKoot[elaimenNimi];
    }


    /// <summary>
    /// Arpoo eläimelle satunnaisen sijainnin pelialueen alaosasta.
    /// </summary>
    /// <param name="olionLeveys">Olion leveys.</param>
    /// <param name="olionKorkeus">Olion korkeus.</param>
    /// <returns>Satunnainen sijainti vektorina.</returns>
    private Vector ArvoSatunnainenSijainti(double olionLeveys, double olionKorkeus)
    {
        double minX = Level.Left + olionLeveys / 2.0;
        double maxX = Level.Right - olionLeveys / 2.0;

        double minY = Level.Bottom + olionKorkeus / 2.0 + ElaimenAlaMarginaali;
        double maxY = Level.Center.Y - olionKorkeus / 2.0;

        double x = RandomGen.NextDouble(minX, maxX);
        double y = RandomGen.NextDouble(minY, maxY);

        return new Vector(x, y);
    }


    /// <summary>
    /// Arpoo eläimelle vapaan sijainnin, joka ei mene päällekkäin muiden eläinten kanssa.
    /// </summary>
    /// <param name="olionLeveys">Olion leveys.</param>
    /// <param name="olionKorkeus">Olion korkeus.</param>
    /// <returns>Vapaa sijainti vektorina.</returns>
    private Vector ArvoVapaaSijainti(double olionLeveys, double olionKorkeus)
    {
        for (int i = 0; i < SpawnYritystenMaara; i++)
        {
            Vector sijainti = ArvoSatunnainenSijainti(olionLeveys, olionKorkeus);

            if (OnkoSijaintiVapaa(sijainti, olionLeveys, olionKorkeus))
            {
                return sijainti;
            }
        }

        return ArvoSatunnainenSijainti(olionLeveys, olionKorkeus);
    }


    /// <summary>
    /// Tarkistaa, onko annettu sijainti vapaa uudelle eläimelle.
    /// </summary>
    /// <param name="sijainti">Tarkistettava sijainti.</param>
    /// <param name="olionLeveys">Olion leveys.</param>
    /// <param name="olionKorkeus">Olion korkeus.</param>
    /// <returns>
    /// Palauttaa true, jos sijainti on vapaa. Muuten false.
    /// </returns>
    private bool OnkoSijaintiVapaa(Vector sijainti, double olionLeveys, double olionKorkeus)
    {
        double vasen = sijainti.X - olionLeveys / 2.0;
        double oikea = sijainti.X + olionLeveys / 2.0;
        double ala = sijainti.Y - olionKorkeus / 2.0;
        double yla = sijainti.Y + olionKorkeus / 2.0;

        foreach (GameObject elain in aktiivisetElaimet)
        {
            if (elain == null) continue;

            bool paallekkain =
                vasen < elain.Right &&
                oikea > elain.Left &&
                ala < elain.Top &&
                yla > elain.Bottom;

            if (paallekkain)
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Hakee eläimen, jonka päällä hiiren klikkaus on.
    /// </summary>
    /// <returns>Klikattu eläin tai null, jos osumaa ei tullut.</returns>
    private GameObject HaeKlikattuElain()
    {
        Vector klikattuSijainti = Mouse.PositionOnWorld;

        for (int i = aktiivisetElaimet.Count - 1; i >= 0; i--)
        {
            GameObject elain = aktiivisetElaimet[i];

            if (elain == null) continue;

            if (klikattuSijainti.X >= elain.Left && klikattuSijainti.X <= elain.Right &&
                klikattuSijainti.Y >= elain.Bottom && klikattuSijainti.Y <= elain.Top)
            {
                return elain;
            }
        }

        return null;
    }


    /// <summary>
    /// Käsittelee pelaajan ampumisen.
    /// </summary>
    private void Ammu()
    {
        if (peliPaattynyt) return;
        if (!peliKaynnissa) return;

        if (laukausAani != null)
        {
            laukausAani.Play();
        }

        GameObject osuttuElain = HaeKlikattuElain();

        if (osuttuElain == null)
        {
            MessageDisplay.Add("Huti");
            return;
        }

        string elaimenNimi = elainTyypit[osuttuElain];

        if (elaimenNimi == "hirvi")
        {
            pisteet.Value += HirviPisteet;

            if (hirviAani != null)
            {
                hirviAani.Play();
            }

            MessageDisplay.Add("Osuma! Hirvi kaadettu.");
        }
        else
        {
            pisteet.Value -= VaaraElainSakko;
            virheet.Value += 1;
            MessageDisplay.Add("Väärä eläin!");

            if (virheet.Value >= MaksimiVirheet)
            {
                PoistaElain(osuttuElain);
                PaataPeliHaviona();
                return;
            }
        }

        PoistaElain(osuttuElain);
    }


    /// <summary>
    /// Poistaa yhden eläimen pelistä ja tietorakenteista.
    /// </summary>
    /// <param name="elain">Poistettava eläin.</param>
    private void PoistaElain(GameObject elain)
    {
        if (elain == null) return;
        if (!elainTyypit.ContainsKey(elain)) return;

        aktiivisetElaimet.Remove(elain);
        elainTyypit.Remove(elain);
        elain.Destroy();
    }


    /// <summary>
    /// Poistaa kaikki eläimet pelistä.
    /// </summary>
    private void PoistaKaikkiElaimet()
    {
        for (int i = aktiivisetElaimet.Count - 1; i >= 0; i--)
        {
            GameObject elain = aktiivisetElaimet[i];

            if (elain != null)
            {
                elain.Destroy();
            }
        }

        aktiivisetElaimet.Clear();
        elainTyypit.Clear();
    }


    /// <summary>
    /// Pysäyttää pelin ajastimet.
    /// </summary>
    private void PysaytaAjastimet()
    {
        if (peliAjastin != null)
        {
            peliAjastin.Stop();
        }

        if (spawnAjastin != null)
        {
            spawnAjastin.Stop();
        }
    }


    /// <summary>
    /// Päättää pelin häviöön.
    /// </summary>
    private void PaataPeliHaviona()
    {
        if (peliPaattynyt) return;

        peliPaattynyt = true;
        PysaytaAjastimet();
        PoistaKaikkiElaimet();

        MessageDisplay.Add("Hävisit! Ammuit kolme väärää eläintä.");
    }


    /// <summary>
    /// Päättää pelin ajan loppuessa.
    /// </summary>
    private void PaataPeliAjanLoppuessa()
    {
        if (peliPaattynyt) return;

        peliPaattynyt = true;
        PysaytaAjastimet();
        PoistaKaikkiElaimet();

        MessageDisplay.Add("Aika loppui! Pisteesi: " + pisteet.Value);
    }
}