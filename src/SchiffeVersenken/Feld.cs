using System;

namespace SchiffeVersenken
{
    public class Feld
    {
        private readonly int x;
        public int X => x;
        private readonly int y;
        public int Y => y;
        private bool markiert;
        public bool Markiert { get => markiert; set => markiert = value; }

        public Feld(int x, int y, bool markiert)
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
}