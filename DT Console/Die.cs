// Albin Bagge
namespace DT_Console;

internal class Die
{
    internal int Value { get; private set; }
    internal const int NrOfSides = 6;

    internal Die()
    {
    }

    // Roll the die & return its value
    internal int Roll()
    {
        Value = Random.Shared.Next(1, NrOfSides + 1);

        return Value;
    }
}
