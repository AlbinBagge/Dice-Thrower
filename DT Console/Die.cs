// Albin Bagge
namespace DT_Console;

internal class Die
{
    internal int Value { get; private set; }

    internal Die()
    {
    }

    // Roll the die & return its value (1-6)
    internal int Roll()
    {
        Value = Random.Shared.Next(1, 7);

        return Value;
    }
}
