using System;
using System.Threading;
using System.Collections.Generic;

namespace SchiffeVersenken
{
    public class SchiffeVersenken
    {
        const int Spielfeldgröße = 10;
        private static readonly Richtung[] Richtungen = (Richtung[])Enum.GetValues(typeof(Richtung));

        private int eingabeX = Spielfeldgröße;
        private int eingabeY = Spielfeldgröße;
        private IList<Schiff> schiffe = new List<Schiff>(7);
        private int abgefeuerteSchüsse = 0;
        private int anzahlTreffer = 0;
        private int anzahlHinweise = 3;
        private bool abbruchAngefordert = false;

        public void Main(string[] args)
        {
            DebugModus.ZeigeUngetroffeneSchiffe = (Environment.GetEnvironmentVariable("DEBUG") == "1");
            int alteFensterBreite = Console.WindowWidth;
            int alteFensterHöhe = Console.WindowHeight;
            string alterFensterTitel = Console.Title;
            Console.SetWindowSize(82, 36);
            Console.Title = "Schiffe versenken";
            SetzeSchiffe();
            AusgabeSpielfeld();
            while (!Ende())
            {
                if (Schießen())
                {
                    AusgabeSpielfeld();
                }
            }
            if (!abbruchAngefordert)
            {
                Console.BackgroundColor = Farbschema.Dialog_Hintergrund;
                Console.ForegroundColor = Farbschema.Meldung_Vordergrund;
                Console.WriteLine("Herzlichen Glückwunsch! Du hast gewonnen!\nBis zum nächsten Mal! Drücke eine Taste.");
            }
            while (!Console.KeyAvailable)
            {
                Thread.Yield();
            }
            Console.SetWindowSize(alteFensterBreite, alteFensterHöhe);
            Console.Title = alterFensterTitel;
            Console.ResetColor();
        }

        private void SetzeSchiffe()
        {
            Random random = new Random();
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.Träger));
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.Schlachtschiff));
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.Kreuzer));
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.Zerstörer));
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.Zerstörer));
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.UBoot));
            schiffe.Add(ErzeugeSchiff(random, Schiffsklasse.UBoot));
        }

        private Schiff ErzeugeSchiff(Random random, Schiffsklasse klasse)
        {
            Schiff schiff = new Schiff(random.Next(Spielfeldgröße), random.Next(Spielfeldgröße), Richtungen[random.Next(Richtungen.Length)], klasse);
            while (schiff.ÜberragtSpielfeld(Spielfeldgröße) || schiff.SchneidetSchiffAusListe(schiffe))
            {
                schiff = new Schiff(random.Next(Spielfeldgröße), random.Next(Spielfeldgröße), Richtungen[random.Next(Richtungen.Length)], klasse);
            }
            return schiff;
        }

        private bool Schießen()
        {
            if (ErfrageKoordinaten())
            {
                ++abgefeuerteSchüsse;
                foreach (Schiff schiff in schiffe)
                {
                    if (schiff.Getroffen(eingabeX, eingabeY))
                    {
                        ++anzahlTreffer;
                    }
                }
                return true;
            }
            return false;
        }

        private bool ErfrageKoordinaten()
        {
            while (true)
            {
                eingabeX = Spielfeldgröße;
                eingabeY = Spielfeldgröße;
                Console.BackgroundColor = Farbschema.Dialog_Hintergrund;
                Console.ForegroundColor = Farbschema.Dialog_Vordergrund;
                Console.WriteLine("Erlaubte Eingaben:");
                Console.WriteLine("A1 - Schuss auf Feld A1");
                Console.WriteLine($"?  - Automatischer Treffer ({anzahlHinweise} übrig)");
                Console.WriteLine("X  - Beenden): ");
                Console.Write("Deine Eingabe: ");
                Console.ForegroundColor = Farbschema.Eingabe_Vordergrund;
                string eingabeStr = Console.ReadLine().Trim().ToLower();
                if (eingabeStr == "x")
                {
                    Console.ForegroundColor = Farbschema.Meldung_Vordergrund;
                    Console.WriteLine("Schade, dass Du schon aufhören willst.\nBis zum nächsten Mal.");
                    abbruchAngefordert = true;
                    return false;
                }
                if (eingabeStr == "?")
                {
                    if (anzahlHinweise > 0)
                    {
                        Random random = new Random();
                        Feld hinweis;
                        while (!schiffe[random.Next(schiffe.Count)].Hilfehinweis(out hinweis)) { }
                        eingabeX = hinweis.X;
                        eingabeY = hinweis.Y;
                        --anzahlHinweise;
                        return true;
                    }
                    Console.ForegroundColor = Farbschema.Fehlermeldung_Vordergrund;
                    Console.WriteLine("Du hast alle Hinweise verbraucht!");
                }
                else
                {
                    if (eingabeStr.Length == 2)
                    {
                        char eingabeXStr = eingabeStr[0];
                        string eingabeYStr = eingabeStr.Substring(1);
                        if (eingabeXStr >= 'a' && eingabeXStr <= 'j')
                        {
                            eingabeX = (int)(eingabeXStr - 'a');
                            int eingabeZahl;
                            if (Int32.TryParse(eingabeYStr, out eingabeZahl) && eingabeZahl >= 0 && eingabeZahl < Spielfeldgröße)
                            {
                                eingabeY = (int)(eingabeZahl == 0 ? 9 : eingabeZahl - 1);
                                return true;
                            }
                        }
                    }
                    Console.ForegroundColor = Farbschema.Fehlermeldung_Vordergrund;
                    Console.WriteLine("Falsche Eingabe (erlaubt sind A1 bis J0)!");
                }
            }
        }

        private bool Ende()
        {
            if (abbruchAngefordert)
            {
                return true;
            }
            foreach (Schiff schiff in schiffe)
            {
                if (!schiff.Versenkt()) return false;
            }
            return true;
        }

        private void AusgabeSpielfeld()
        {
            Console.Clear();
            Console.BackgroundColor = Farbschema.Dialog_Hintergrund;
            Console.ForegroundColor = Farbschema.Dialog_Vordergrund;
            Console.WriteLine("        Willkommen bei Schiffe versenken");
            Console.WriteLine("        ================================\n");

            IDictionary<Schiffsklasse, Int32> schiffslegende = new Dictionary<Schiffsklasse, Int32>();
            foreach (Schiff schiff in schiffe)
            {
                int anzahl;
                schiffslegende.TryGetValue(schiff.Klasse, out anzahl);
                if (!schiff.Versenkt())
                {
                    ++anzahl;
                }
                schiffslegende[schiff.Klasse] = anzahl;
            }
            IEnumerator<KeyValuePair<Schiffsklasse, int>> schiffslegendeIterator = schiffslegende.GetEnumerator();

            // Kopfzeile
            SchreibeLegendeX(true);

            for (int y = 0; y < Spielfeldgröße; ++y)
            {
                // Kopfspalte
                Console.BackgroundColor = Farbschema.Rahmen_Hintergrund;
                Console.ForegroundColor = Farbschema.Rahmen_Vordergrund;
                Console.Write($"║ {(y + 1) % 10} ║");

                // Spielfeld
                Console.BackgroundColor = Farbschema.Spielfeld_Hintergrund;
                Console.ForegroundColor = Farbschema.Spielfeld_Vordergrund;
                for (int x = 0; x < Spielfeldgröße; ++x)
                {
                    Console.BackgroundColor = Farbschema.Spielfeld_Hintergrund;
                    char anzeige = Anzeige.Wasser;
                    if (eingabeX == x && eingabeY == y)
                    {
                        Console.ForegroundColor = Farbschema.Spielfeld_Treffer_Vordergrund;
                        anzeige = Anzeige.Treffer_Wasser;
                        foreach (Schiff schiff in schiffe)
                        {
                            if (schiff.Getroffen(x, y))
                            {
                                Console.BackgroundColor = Farbschema.Spielfeld_Schiff_Hintergrund;
                                if (schiff.Versenkt())
                                {
                                    anzeige = schiff.Klasse.ToString()[0];
                                }
                                else
                                {
                                    anzeige = Anzeige.Treffer_Schiff;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Schiff schiff in schiffe)
                        {
                            if (schiff.Sichtbar(x, y))
                            {
                                Console.BackgroundColor = Farbschema.Spielfeld_Schiff_Hintergrund;
                                Console.ForegroundColor = Farbschema.Spielfeld_alte_Züge_Vordergrund;
                                anzeige = schiff.Versenkt() ? schiff.Klasse.ToString()[0] : Anzeige.Schiff_sichtbar;
                            }
                        }
                    }
                    Console.Write($" {anzeige} ");
                    Console.BackgroundColor = Farbschema.Spielfeld_Hintergrund;
                    Console.ForegroundColor = Farbschema.Spielfeld_Vordergrund;
                    switch (x)
                    {
                        case 4:
                            Console.Write('║');
                            break;
                        case Spielfeldgröße - 1:
                            break;
                        default:
                            Console.Write('│');
                            break;
                    }
                    Console.ForegroundColor = Farbschema.Spielfeld_Vordergrund;
                }

                // Kopfspalte
                Console.BackgroundColor = Farbschema.Rahmen_Hintergrund;
                Console.ForegroundColor = Farbschema.Rahmen_Vordergrund;
                Console.Write($"║ {(y + 1) % Spielfeldgröße} ║");

                Console.BackgroundColor = Farbschema.Dialog_Hintergrund;
                Console.ForegroundColor = Farbschema.Dialog_Vordergrund;
                Console.Write("   ");

                switch (y)
                {
                    case 0:
                        Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                        Console.ForegroundColor = Farbschema.Statistik_Überschrift_Vordergrund;
                        Console.Write($"{"  Verborgene Schiffe (Länge)",-30}");
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        if (schiffslegendeIterator.MoveNext())
                        {
                            Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                            Console.ForegroundColor = Farbschema.Statistik_Vordergrund;
                            Console.Write($"{schiffslegendeIterator.Current.Key,17} ({(int)schiffslegendeIterator.Current.Key}) : {schiffslegendeIterator.Current.Value,3}   ");
                        }
                        break;
                    case 6:
                        Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                        Console.ForegroundColor = Farbschema.Statistik_Überschrift_Vordergrund;
                        Console.Write($"{"  Statistik",-30}");
                        break;
                    case 7:
                        Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                        Console.ForegroundColor = Farbschema.Statistik_Vordergrund;
                        Console.Write($"{"  abgegebene Schüsse",21} : {abgefeuerteSchüsse,3}   ");
                        break;
                    case 8:
                        Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                        Console.ForegroundColor = Farbschema.Statistik_Vordergrund;
                        Console.Write($"{"Trefferquote",21} : {(int)(abgefeuerteSchüsse == 0 ? 0 : anzahlTreffer * 100 / abgefeuerteSchüsse),3}%  ");
                        break;
                    case 9:
                        Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                        Console.ForegroundColor = Farbschema.Statistik_Vordergrund;
                        Console.Write($"{"  genutzte Hinweise",21} : {3 - anzahlHinweise,3}   ");
                        break;
                }
                Console.WriteLine("");

                if (y < Spielfeldgröße - 1)
                {
                    // Trennzeile
                    Console.BackgroundColor = Farbschema.Rahmen_Hintergrund;
                    Console.ForegroundColor = Farbschema.Rahmen_Vordergrund;
                    Console.Write("╠═══╬");
                    Console.BackgroundColor = Farbschema.Spielfeld_Hintergrund;
                    Console.ForegroundColor = Farbschema.Spielfeld_Vordergrund;
                    Console.Write("═══╪═══╪═══╪═══╪═══╬═══╪═══╪═══╪═══╪═══");
                    Console.BackgroundColor = Farbschema.Rahmen_Hintergrund;
                    Console.ForegroundColor = Farbschema.Rahmen_Vordergrund;
                    Console.Write("╬═══╣");
                    Console.BackgroundColor = Farbschema.Dialog_Hintergrund;
                    Console.ForegroundColor = Farbschema.Dialog_Vordergrund;
                    Console.Write("   ");
                    Console.BackgroundColor = Farbschema.Statistik_Hintergrund;
                    Console.ForegroundColor = Farbschema.Statistik_Vordergrund;
                    Console.WriteLine($"{" ",30}");
                }
            }
            // Fußzeile
            SchreibeLegendeX(false);
            Console.WriteLine("");
        }

        private static void SchreibeLegendeX(bool istKopf)
        {
            Console.BackgroundColor = Farbschema.Rahmen_Hintergrund;
            Console.ForegroundColor = Farbschema.Rahmen_Vordergrund;
            if (istKopf)
            {
                Console.WriteLine("╔═══╦═══╤═══╤═══╤═══╤═══╦═══╤═══╤═══╤═══╤═══╦═══╗");
            }
            else
            {
                Console.WriteLine("╠═══╬═══╪═══╪═══╪═══╪═══╬═══╪═══╪═══╪═══╪═══╬═══╣");
            }
            Console.WriteLine("║   ║ A │ B │ C │ D │ E ║ F │ G │ H │ I │ J ║   ║");
            if (istKopf)
            {
                Console.WriteLine("╠═══╬═══╪═══╪═══╪═══╪═══╬═══╪═══╪═══╪═══╪═══╬═══╣");
            }
            else
            {
                Console.WriteLine("╚═══╩═══╧═══╧═══╧═══╧═══╩═══╧═══╧═══╧═══╧═══╩═══╝");
            }
        }
    }
}