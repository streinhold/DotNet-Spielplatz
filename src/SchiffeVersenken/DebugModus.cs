namespace SchiffeVersenken
{
    internal class DebugModus
    {
        private static bool zeigeUngetroffeneSchiffe = false;

        internal static bool ZeigeUngetroffeneSchiffe { get => zeigeUngetroffeneSchiffe; set => zeigeUngetroffeneSchiffe = value; }
    }
}