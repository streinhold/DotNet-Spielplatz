using System;
using System.Threading;
using System.Collections.Generic;

namespace SchiffeVersenken
{
    internal class DebugModus
    {
        private static bool zeigeUngetroffeneSchiffe = false;

        internal static bool ZeigeUngetroffeneSchiffe { get => zeigeUngetroffeneSchiffe; set => zeigeUngetroffeneSchiffe = value; }
    }

    internal enum Richtung : byte
    {
        NACH_OBEN = 0b_0000_1111,
        NACH_RECHTS = 0b_0001_0000,
        NACH_UNTEN = 0b_0000_0001,
        NACH_LINKS = 0b_1111_0000
    }

    internal enum Schiffsklasse : int
    {
        Träger = 5,
        Schlachtschiff = 4,
        Kreuzer = 3,
        Zerstörer = 2,
        UBoot = 1
    }

    internal class Feld
    {
        private readonly int x;
        private readonly int y;
        private bool markiert;

        internal bool Markiert { get => markiert; set => markiert = value; }

        internal int X => x;

        internal int Y => y;

        internal Feld(int x, int y, bool markiert)
        {
            this.x = x;
            this.y = y;
            this.markiert = markiert;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Feld feld = (Feld)obj;
            return feld.X == this.X && feld.Y == this.Y;
        }

        // overload Equals
        public bool Equals(Feld feld) => feld != null && feld.X == this.X && feld.Y == this.Y;

        // override == operator
        public static bool operator ==(Feld feldA, Feld feldB)
        {
            if (Object.ReferenceEquals(feldA, feldB)) return true;
            if (Object.ReferenceEquals(null, feldA)) return false;
            return feldA.Equals(feldB);
        }

        // override != operator
        public static bool operator !=(Feld feldA, Feld feldB) => !(feldA == feldB);

        // override object.GetHashCode
        public override int GetHashCode() => X << 4 & Y;

        // override object.ToString
        public override string ToString() => $"({x}, {y}) => {markiert}";
    }

    internal class Schiff
    {
        private IList<Feld> positionen;
        private readonly Schiffsklasse klasse;
        internal Schiffsklasse Klasse => klasse;

        private int unbeschädigteSektionen;

        internal Schiff(int startX, int startY, Richtung richtung, Schiffsklasse klasse)
        {
            this.klasse = klasse;
            unbeschädigteSektionen = (int)klasse;
            int deltaX = (int)richtung << 24 >> 28;
            int deltaY = (int)richtung << 28 >> 28;
            positionen = new List<Feld>(unbeschädigteSektionen);
            if (deltaX == 0)
            {
                for (int y = 0; y < unbeschädigteSektionen; ++y)
                {
                    positionen.Add(new Feld(startX, startY + deltaY * y, false));
                }
            }
            else // deltaY == 0
            {
                for (int x = 0; x < unbeschädigteSektionen; ++x)
                {
                    positionen.Add(new Feld(startX + deltaX * x, startY, false));
                }
            }
        }

        internal bool ÜberragtSpielfeld(int größe)
        {
            foreach (Feld feld in positionen)
            {
                if (feld.X < 0 || feld.X >= größe || feld.Y < 0 || feld.Y >= größe)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool SchneidetSchiffAusListe(IList<Schiff> andereSchiffe)
        {
            foreach (Schiff schiff in andereSchiffe)
            {
                if (Schneidet(schiff))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool Schneidet(Schiff anderesSchiff)
        {
            foreach (Feld eigenesFeld in positionen)
            {
                foreach (Feld anderesFeld in anderesSchiff.positionen)
                {
                    if (anderesFeld == eigenesFeld)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool Getroffen(int x, int y)
        {
            Feld gesucht = new Feld(x, y, true);
            foreach (Feld element in positionen)
            {
                if (element == gesucht)
                {
                    // erneuter Treffer an einer bereits getroffenen Stelle
                    if (!element.Markiert)
                    {
                        --unbeschädigteSektionen;
                    }
                    element.Markiert = true;
                    return true;
                }
            }
            return false;
        }
        internal bool Versenkt() => unbeschädigteSektionen == 0;

        internal bool Sichtbar(int x, int y)
        {
            foreach (Feld feld in positionen)
            {
                if (feld.X == x && feld.Y == y && (feld.Markiert || DebugModus.ZeigeUngetroffeneSchiffe))
                {
                    return true;
                }
            }
            return false;
        }

        // override object.ToString
        public override string ToString()
        {
            return $"${klasse} ({positionen[0].X},{positionen[0].Y}) - ({positionen[positionen.Count - 1].X}, {positionen[positionen.Count - 1].Y} {(Versenkt() ? "versenkt" : "schwimmt")}";
        }
    }

    internal class Anzeige
    {
        internal const char WASSER = '~';
        internal const char SCHIFF_UNSICHTBAR = 's';
        internal const char SCHIFF_SICHTBAR = 'S';
        internal const char SCHIFF_VERSENKT = 'V';
        internal const char TREFFER_WASSER = '*';
        internal const char TREFFER_SCHIFF = 'X';
    }

    public class SchiffeVersenken
    {
        const ConsoleColor RAHMEN_HINTERGRUND = ConsoleColor.DarkGray;
        const ConsoleColor RAHMEN_VORDERGRUND = ConsoleColor.Gray;
        const ConsoleColor SPIELFELD_HINTERGRUND = ConsoleColor.Blue;
        const ConsoleColor SPIELFELD_VORDERGRUND = ConsoleColor.DarkBlue;
        const ConsoleColor SPIELFELD_SCHIFF_HINTERGRUND = ConsoleColor.DarkRed;
        const ConsoleColor SPIELFELD_TREFFER_VORDERGRUND = ConsoleColor.Yellow;
        const ConsoleColor SPIELFELD_ALTE_ZÜGE_VORDERGRUND = ConsoleColor.White;
        const ConsoleColor DIALOG_HINTERGRUND = ConsoleColor.Black;
        const ConsoleColor DIALOG_VORDERGRUND = ConsoleColor.White;
        const ConsoleColor EINGABE_VORDERGRUND = ConsoleColor.Green;
        const ConsoleColor MELDUNG_VORDERGRUND = ConsoleColor.Yellow;
        const ConsoleColor FEHLERMELDUNG_VORDERGRUND = ConsoleColor.Red;
        const int DIMENSION = 10;

        private int eingabeX = DIMENSION;
        private int eingabeY = DIMENSION;
        private IList<Schiff> schiffe = new List<Schiff>(7);
        private int abgefeuerteSchüsse = 0;

        public void Main(string[] args)
        {
            DebugModus.ZeigeUngetroffeneSchiffe = (Environment.GetEnvironmentVariable("DEBUG") == "1");
            SetzeSchiffe();
            AusgabeSpielfeld();
            while (!Ende())
            {
                Schießen();
                AusgabeSpielfeld();
            }
            Console.BackgroundColor = DIALOG_HINTERGRUND;
            Console.ForegroundColor = MELDUNG_VORDERGRUND;
            Console.WriteLine("Herzlichen Glückwunsch! Du hast gewonnen!\nBis zum nächsten Mal! Drücke eine Taste.");
            while (!Console.KeyAvailable)
            {
                Thread.Yield();
            }
            Console.ResetColor();
        }

        private readonly Richtung[] richtungen = (Richtung[])Enum.GetValues(typeof(Richtung));

        private Schiff ErzeugeSchiff(Random random, Schiffsklasse klasse)
        {
            Schiff schiff = new Schiff(random.Next(DIMENSION), random.Next(DIMENSION), richtungen[random.Next(richtungen.Length)], klasse);
            while (schiff.ÜberragtSpielfeld(DIMENSION) || schiff.SchneidetSchiffAusListe(schiffe))
            {
                schiff = new Schiff(random.Next(DIMENSION), random.Next(DIMENSION), richtungen[random.Next(richtungen.Length)], klasse);
            }
            return schiff;
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

        private void Schießen()
        {
            ErfrageKoordinate();
            ++abgefeuerteSchüsse;
            foreach (Schiff schiff in schiffe)
            {
                schiff.Getroffen(eingabeX, eingabeY);
            }
        }

        private void ErfrageKoordinate()
        {
            while (true)
            {
                eingabeX = DIMENSION;
                eingabeY = DIMENSION;
                Console.BackgroundColor = DIALOG_HINTERGRUND;
                Console.ForegroundColor = DIALOG_VORDERGRUND;
                Console.Write("Wohin willst Du schießen? (X für Ende): ");
                Console.ForegroundColor = EINGABE_VORDERGRUND;
                string eingabeStr = Console.ReadLine().Trim().ToLower();
                if (eingabeStr == "x")
                {
                    Console.ForegroundColor = MELDUNG_VORDERGRUND;
                    Console.WriteLine("Schade, dass Du schon aufhören willst.\nBis zum nächsten Mal.");
                    Console.ResetColor();
                    Environment.Exit(0);
                }
                if (eingabeStr.Length == 2)
                {
                    char eingabeXStr = eingabeStr[0];
                    string eingabeYStr = eingabeStr.Substring(1);
                    if (eingabeXStr >= 'a' && eingabeXStr <= 'j')
                    {
                        eingabeX = (int)(eingabeXStr - 'a');
                        int eingabeZahl;
                        if (Int32.TryParse(eingabeYStr, out eingabeZahl) && eingabeZahl >= 0 && eingabeZahl < DIMENSION)
                        {
                            eingabeY = (int)(eingabeZahl == 0 ? 9 : eingabeZahl - 1);
                            return;
                        }
                    }
                }
                Console.ForegroundColor = FEHLERMELDUNG_VORDERGRUND;
                Console.WriteLine("\u000aFalsche Eingabe (erlaubt sind A1 bis J0)!");
            }
        }

        private bool Ende()
        {
            foreach (Schiff schiff in schiffe)
            {
                if (!schiff.Versenkt()) return false;
            }
            return true;
        }

        private void AusgabeSpielfeld()
        {
            Console.Clear();
            Console.BackgroundColor = DIALOG_HINTERGRUND;
            Console.ForegroundColor = DIALOG_VORDERGRUND;
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

            for (int y = 0; y < DIMENSION; ++y)
            {
                // Kopfspalte
                Console.BackgroundColor = RAHMEN_HINTERGRUND;
                Console.ForegroundColor = RAHMEN_VORDERGRUND;
                Console.Write($"║ {(y + 1) % 10} ║");

                // Spielfeld
                Console.BackgroundColor = SPIELFELD_HINTERGRUND;
                Console.ForegroundColor = SPIELFELD_VORDERGRUND;
                for (int x = 0; x < DIMENSION; ++x)
                {
                    Console.BackgroundColor = SPIELFELD_HINTERGRUND;
                    char anzeige = Anzeige.WASSER;
                    if (eingabeX == x && eingabeY == y)
                    {
                        Console.ForegroundColor = SPIELFELD_TREFFER_VORDERGRUND;
                        anzeige = Anzeige.TREFFER_WASSER;
                        foreach (Schiff schiff in schiffe)
                        {
                            if (schiff.Getroffen(x, y))
                            {
                                Console.BackgroundColor = SPIELFELD_SCHIFF_HINTERGRUND;
                                if (schiff.Versenkt())
                                {
                                    anzeige = Anzeige.SCHIFF_VERSENKT;
                                }
                                else
                                {
                                    anzeige = Anzeige.TREFFER_SCHIFF;
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
                                Console.BackgroundColor = SPIELFELD_SCHIFF_HINTERGRUND;
                                Console.ForegroundColor = SPIELFELD_ALTE_ZÜGE_VORDERGRUND;
                                anzeige = schiff.Versenkt() ? Anzeige.SCHIFF_VERSENKT : Anzeige.SCHIFF_SICHTBAR;
                            }
                        }
                    }
                    Console.Write($" {anzeige} ");
                    Console.BackgroundColor = SPIELFELD_HINTERGRUND;
                    Console.ForegroundColor = SPIELFELD_VORDERGRUND;
                    switch (x)
                    {
                        case 4:
                            Console.Write('║');
                            break;
                        case DIMENSION - 1:
                            break;
                        default:
                            Console.Write('│');
                            break;
                    }
                    Console.ForegroundColor = SPIELFELD_VORDERGRUND;
                }

                // Kopfspalte
                Console.BackgroundColor = RAHMEN_HINTERGRUND;
                Console.ForegroundColor = RAHMEN_VORDERGRUND;
                Console.Write($"║ {(y + 1) % DIMENSION} ║");

                switch (y)
                {
                    case 0:
                        Console.BackgroundColor = DIALOG_HINTERGRUND;
                        Console.ForegroundColor = DIALOG_VORDERGRUND;
                        Console.Write($"{"Versteckte Schiffe",24}");
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        if (schiffslegendeIterator.MoveNext())
                        {
                            Console.BackgroundColor = DIALOG_HINTERGRUND;
                            Console.ForegroundColor = DIALOG_VORDERGRUND;
                            Console.Write($"{schiffslegendeIterator.Current.Key,20} : {schiffslegendeIterator.Current.Value,2}");
                        }
                        break;
                    case 8:
                        Console.BackgroundColor = DIALOG_HINTERGRUND;
                        Console.ForegroundColor = DIALOG_VORDERGRUND;
                        Console.Write($"{"Schüsse",20} : {abgefeuerteSchüsse,2}");
                        break;
                }
                Console.WriteLine("");

                if (y < DIMENSION - 1)
                {
                    // Trennzeile
                    Console.BackgroundColor = RAHMEN_HINTERGRUND;
                    Console.ForegroundColor = RAHMEN_VORDERGRUND;
                    Console.Write("╠═══╬");
                    Console.BackgroundColor = SPIELFELD_HINTERGRUND;
                    Console.ForegroundColor = SPIELFELD_VORDERGRUND;
                    Console.Write("═══╪═══╪═══╪═══╪═══╬═══╪═══╪═══╪═══╪═══");
                    Console.BackgroundColor = RAHMEN_HINTERGRUND;
                    Console.ForegroundColor = RAHMEN_VORDERGRUND;
                    Console.WriteLine("╬═══╣");
                }
            }
            // Fußzeile
            SchreibeLegendeX(false);
            Console.WriteLine("");
        }

        private static void SchreibeLegendeX(bool istKopf)
        {
            Console.BackgroundColor = RAHMEN_HINTERGRUND;
            Console.ForegroundColor = RAHMEN_VORDERGRUND;
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