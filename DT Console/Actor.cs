// Albin Bagge
namespace DT_Console;

public abstract class Actor
{
    public string Name { get; set; }

    public Actor()
    {
        Name = string.Empty;
        Console.WriteLine("Debug: Actor");
    }
}
