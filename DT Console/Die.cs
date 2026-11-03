namespace DT_Console;

public class Die
{
    public int Value { get; private set; }

    public Die()
    {
    }

    // Roll the dice, then print & return its value
    public int Roll()
    {
        Value = Random.Shared.Next(1, 7);

        return Value;
    }
}
