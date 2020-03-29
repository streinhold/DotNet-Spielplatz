namespace SchiffeVersenken
{
    /*
    Die vier Richtungen (ausgehend von einem Startpunkt)
    sind als Änderung in x- und y-Richtung definiert.
    Der Nullpunkt (0, 0) ist dabei oben links!
    Das obere Halbbyte definiert die Änderung von X (-1, 0, 1),
    das untere Halbbyte definiert die Änderung von Y (-1, 0, 1).

    Damit man daraus die Änderung als int bekommt, wird das Byte erst
    links verschoben, um das Vorzeichenbit an die erste Stelle zu
    transferieren und danach rechts verschoben, so dass das unterste
    Bit des entsprechenden Halbbytes an Stelle von Bit 0 steht.
    */
    public enum Richtung : byte
    {
        NACH_OBEN = 0b_0000_1111,
        NACH_RECHTS = 0b_0001_0000,
        NACH_UNTEN = 0b_0000_0001,
        NACH_LINKS = 0b_1111_0000
    }

    public static class Extensions
    {
        public static int DeltaX(this Richtung richtung)
        {
            return (int)richtung << 24 >> 28;
        }
        public static int DeltaY(this Richtung richtung)
        {
            return (int)richtung << 28 >> 28;
        }
    }
}