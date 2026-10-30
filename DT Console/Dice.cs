namespace DT_Console;

public class Dice
{
    public int Value { get; private set; }

    private string _ownerName;

    public Dice(string ownerName)
    {
        _ownerName = ownerName;
    }

    // Roll the dice, then print & return its value
    public int RollDice()
    {
        Value = Random.Shared.Next(1, 7);
        Console.WriteLine($"{_ownerName} rolled a {Value}.");

        return Value;
    }
}
