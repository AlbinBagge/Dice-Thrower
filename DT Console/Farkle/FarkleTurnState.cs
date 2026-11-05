// Albin Bagge
namespace DT_Console;

internal class FarkleTurnState
{
    internal Actor Actor { get; }
    internal List<Die> HandDice { get; } = new();
    internal List<Die> SelectedDice { get; } = new();
    //internal List<Die> ScoringDice { get; } = new(); // Todo: remove?
    internal List<Die> RemainingDice { get; } = new();
    internal int TempScore { get; set; } = 0;
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
            HandDice.Add(new Die());
        }
    }
}
