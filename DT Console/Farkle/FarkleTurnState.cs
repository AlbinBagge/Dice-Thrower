// Albin Bagge
namespace DT_Console;

internal class FarkleTurnState
{
    internal Actor Actor { get; }
    internal List<Die> RemainingDice { get; } = new();
    internal List<Die> SetAsideDice { get; } = new();
    internal int Score { get; set; } = 0;
    internal bool IsActive { get; set; } = true;
    private const int DicePerHand = 6;

    internal FarkleTurnState(Actor actor)
    {
        Actor = actor;

        AddDice();
    }

    internal void AddDice()
    {
        for (int i = 0; i < DicePerHand; i++)
        {
            RemainingDice.Add(new Die());
        }
    }
}
