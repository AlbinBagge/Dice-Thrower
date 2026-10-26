// Albin Bagge
namespace DT_Console;

public class Player : Actor
{
    public Player(string name)
    {
        Name = name;
        Console.WriteLine($"Debug: Player \"{Name}\"");
    }
}
