// Albin Bagge
namespace DT_Console;

public static class Utilities
{
    static Utilities() // Todo: No constructor needed?
    {
    }

    // Pause & prompt the user to press a key
    public static void WaitForKeyPress()
    {
        Console.Write("Press any key");
        Console.ReadKey();
    }

    // Print current players & opponents
    public static void PrintActors(List<Player> players, List<Opponent> opponents)
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
}
