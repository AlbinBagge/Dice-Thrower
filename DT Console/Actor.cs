// Albin Bagge
namespace DT_Console;

internal abstract class Actor
{
    internal string Name { get; set; } = string.Empty;
    internal int Score { get; set; } = 0;

    internal Actor()
    {
    }
}
