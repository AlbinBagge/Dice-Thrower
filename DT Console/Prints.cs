// Albin Bagge
namespace DT_Console;

internal static class Prints
{
    // Pause & prompt the user to press a key
    internal static void WaitForKeyPress()
    {
        Console.Write("Press any key");
        Console.ReadKey();
    }

    // Print current players & opponents
    internal static void PrintActors(List<Player> players, List<Opponent> opponents)
    {
        Console.WriteLine("Current players:");
        foreach (Player player in players)
        {
            Console.WriteLine($"- {player.Name}");
        }
        Console.WriteLine();
        Console.WriteLine("Current AI opponents:");
        foreach (Opponent opponent in opponents)
        {
            Console.WriteLine($"- {opponent.Name}");
        }
    }

    internal static void PrintDieInColor(Die die, ConsoleColor backgroundColor)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = backgroundColor;
        Console.Write($" {die.Value} ");
        Console.ResetColor();
    }

    internal static void PrintDiceList(List<Die> dice)
    {
        foreach (Die die in dice)
        {
            PrintDieInColor(die, ConsoleColor.Yellow);
            Console.Write(" ");
        }
    }

    internal static void WriteInColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
}
