using System;

namespace DotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            string wahl;
            if (args.Length == 1)
            {
                wahl = args[0];
                switch (wahl)
                {
                    case "1":
                        new SchiffeVersenken.SchiffeVersenken().Main(args);
                        return;
                }
            }
            else
            {
                do
                {
                    Console.WriteLine("Was willst Du sehen?");
                    Console.WriteLine("1 - Schiffe versenken");
                    Console.WriteLine("E - Ende");
                    Console.Write("Deine Wahl: ");
                    wahl = Console.ReadLine();
                    switch (wahl)
                    {
                        case "1":
                            new SchiffeVersenken.SchiffeVersenken().Main(args);
                            return;
                        case "E":
                        case "e":
                            Console.WriteLine("Bis zum nächsten Mal.");
                            return;
                        default:
                            Console.WriteLine("Ungültige Wahl! Bitte gib das Zeichen am Anfang der Zeile ein.");
                            break;
                    }
                } while (true);
            }
        }
    }
}
