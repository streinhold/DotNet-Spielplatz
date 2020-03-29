using System;
using System.Collections.Generic;

namespace SchiffeVersenken
{
    public class Schiff
    {
        private IList<Feld> positionen;
        private readonly Schiffsklasse klasse;
        public Schiffsklasse Klasse => klasse;

        private int unbeschädigteSektionen;

        public Schiff(int startX, int startY, Richtung richtung, Schiffsklasse klasse)
        {
            this.klasse = klasse;
            unbeschädigteSektionen = (int)klasse;
            int deltaX = richtung.DeltaX();
            int deltaY = richtung.DeltaY();
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

        public bool ÜberragtSpielfeld(int größe)
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

        public bool SchneidetSchiffAusListe(IList<Schiff> andereSchiffe)
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

        private bool Schneidet(Schiff anderesSchiff)
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

        public bool Getroffen(int x, int y)
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
        public bool Versenkt() => unbeschädigteSektionen == 0;

        public bool Sichtbar(int x, int y)
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

        /**
           <summary>Liefert ein zufälliges ungetroffenes Feld als Hinweis.</summary>
           <param name="ungetroffenesFeld">das Feld für den Hilfehinweis oder <c>null</c>, falls es kein ungetroffenes Feld mehr gibt</param>
           <returns><c>true</c>, falls es einen Hinweis gibt, sonst <c>false</c></returns>
         */
        public bool Hilfehinweis(out Feld ungetroffenesFeld)
        {
            if (Versenkt())
            {
                ungetroffenesFeld = null;
                return false;
            }
            int hinweisIndex = new Random().Next(unbeschädigteSektionen);
            foreach (Feld feld in positionen)
            {
                if (!feld.Markiert)
                {
                    if (hinweisIndex-- == 0)
                    {
                        ungetroffenesFeld = feld;
                        return true;
                    }
                }
            }
            ungetroffenesFeld = null;
            return false;
        }

        // override object.ToString
        public override string ToString()
        {
            return $"${klasse} ({positionen[0].X},{positionen[0].Y}) - ({positionen[positionen.Count - 1].X}, {positionen[positionen.Count - 1].Y} {(Versenkt() ? "versenkt" : "schwimmt")}";
        }
    }
}