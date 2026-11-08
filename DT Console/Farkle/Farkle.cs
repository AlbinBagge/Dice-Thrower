// Albin Bagge
namespace DT_Console;

internal class Farkle
{
    private readonly List<Player> _players;
    private readonly List<Opponent> _opponents;
    private readonly List<Actor> _actors = new();
    private readonly FarkleRules _rules = new();
    private const int WinScore = 4000;

    internal Farkle(List<Player> players, List<Opponent> opponents)
    {
        _players = players;
        _opponents = opponents;

        // Add players & opponents to one actor list
        _actors.AddRange(players);
        _actors.AddRange(opponents);
    }

    internal void StartGame()
    {
        bool gameIsActive = true;

        PrintFarkleScreen();

        // Game loop
        do
        {
            // Round
            foreach (Actor actor in _actors)
            {
                StartTurn(actor);
            }

            // Win condition
            if (_actors.Any(a => a.Score >= WinScore))
            {
                PrintWinner();

                // Reset scores
                foreach (Actor actor in _actors)
                {
                    actor.Score = 0;
                }

                // Game over
                gameIsActive = false;
            }
        }
        while (gameIsActive);
    }

    private void StartTurn(Actor actor)
    {
        // Handle information needed for the turn
        FarkleTurnState turn = new(actor);

        do
        {
            RollAndPrintHand(turn);

            if (_rules.HasScore(turn.HandDice))
            {
                ChooseSetAsideOrStand(turn);

                // Update the number of non-scoring dice remaining in the hand
                _rules.ScoreDice(turn, turn.HandDice, false);

                // Check if all dice in the hand scored
                if (turn.RemainingDice.Count() == 0)
                {
                    ChooseContinueOrEndTurn(turn);
                }
            }
            else
            {
                EndTurnWithFarkle(turn);
            }
        }
        while (turn.IsActive);
    }

    private void PrintFarkleScreen()
    {
        Console.Clear();

        // Title
        Console.WriteLine("> Dice Thrower > Farkle");
        Console.WriteLine();

        // List of Players & Opponents
        Prints.PrintActors(_players, _opponents);
        Console.WriteLine();
    }

    private void RollAndPrintHand(FarkleTurnState turn)
    {
        // Throw current hand of dice
        foreach (Die die in turn.HandDice)
        {
            die.Roll();
        }

        // Sort dice by value for better presentation
        turn.HandDice.Sort((first, second) => first.Value.CompareTo(second.Value));

        // Print rolled dice
        Console.Write($"{turn.Actor.Name} rolled: ");
        Prints.PrintDiceList(turn.HandDice);
        Console.WriteLine();
    }

    private void ChooseSetAsideOrStand(FarkleTurnState turn)
    {
        Console.WriteLine(
                    $"1. Set aside\n" +
                    $"2. Stand");

        bool menuIsActive = true;
        do
        {
            // Choose: set aside / stand
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    SetAsideDice(turn);
                    menuIsActive = false;
                    break;

                case ConsoleKey.D2:
                    Stand(turn);
                    menuIsActive = false;
                    break;
            }
        }
        while (menuIsActive);
    }

    private void ChooseContinueOrEndTurn(FarkleTurnState turn)
    {
        Prints.WriteInColor($"All of {turn.Actor.Name}'s dice scored!\n", ConsoleColor.Green);
        Console.WriteLine(
            $"Do they wish to continue playing their turn with new dice?\n" +
            $"1. Yes\n" +
            $"2. No");

        bool menuIsActive = true;
        do
        {
            // Choose: continue turn / end turn
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    // Print choice
                    Console.WriteLine("YES");
                    Console.WriteLine();

                    // Continue the turn
                    turn.IsActive = true;

                    // Reset dice
                    turn.HandDice.Clear();
                    turn.AddDice();

                    menuIsActive = false;
                    break;

                case ConsoleKey.D2:
                    // Print choice
                    Console.WriteLine("NO");
                    Console.WriteLine();

                    // End the turn
                    turn.IsActive = false;

                    menuIsActive = false;
                    break;
            }
        }
        while (menuIsActive);
    }

    private void SetAsideDice(FarkleTurnState turn)
    {
        // Print choice
        Console.WriteLine(
            "SET ASIDE - Input the values to set aside, with or without spaces, " +
            "and press Enter:");

        bool isSettingAside = true;
        do
        {
            // Reset selected dice & their score
            turn.SelectedDice.Clear();
            turn.TempScore = 0;

            List<int> values = InputValues();

            // Make sure the list of requested values isn't empty
            if (values.Count == 0)
            {
                continue;
            }

            // Make sure the hand contains all requested values
            if (!_rules.DiceContainsValues(turn.HandDice, values))
            {
                continue;
            }

            SelectDice(turn, values);

            // Obtain the score from selected dice
            turn.TempScore = _rules.ScoreDice(turn, turn.SelectedDice, true);

            // Check if there are non-scoring dice remaining from selected dice
            // (Non-scoring dice can not be set aside)
            if (turn.RemainingDice.Count() != 0)
            {
                continue;
            }
            Console.WriteLine();

            // Add score from selected dice
            turn.Score += turn.TempScore;

            RemoveSelectionFromHand(turn);

            isSettingAside = false;
        }
        while (isSettingAside);
    }

    private void Stand(FarkleTurnState turn)
    {
        // Print choice
        Console.WriteLine("STAND");

        // Add final turn score to actor's core
        turn.Score += _rules.ScoreDice(turn, turn.HandDice, true);
        turn.Actor.Score += turn.Score;

        // Print scores
        Console.Write($"{turn.Actor.Name} stands. (TURN: ");
        Prints.WriteInColor($"{turn.Score}", ConsoleColor.Yellow);
        Console.Write($" score / TOTAL: ");
        Prints.WriteInColor($"{turn.Actor.Score}", ConsoleColor.Yellow);
        Console.WriteLine(" score)");
        Console.WriteLine();

        // End the turn
        turn.IsActive = false;
    }

    private void PrintWinner()
    {
        // Get actor with the highest score (account for null value)
        Actor? winner = _actors.MaxBy(actor => actor.Score);

        if (winner != null)
        {
            Prints.WriteInColor($"The winner is {winner.Name} with {winner.Score} score!\n",
                ConsoleColor.Green);
            Console.WriteLine();
            Prints.WaitForKeyPress();
        }
    }

    private void EndTurnWithFarkle(FarkleTurnState turn)
    {
        // Farkle (end turn without score)
        turn.IsActive = false;
        Prints.WriteInColor($"{turn.Actor.Name} farkled! (No score.)\n", ConsoleColor.Red);
        Console.WriteLine();
    }

    private List<int> InputValues()
    {
        // Input string for dice selection
        string valuesText;
        do
        {
            valuesText = Console.ReadLine() ?? string.Empty;
        }
        while (string.IsNullOrWhiteSpace(valuesText));

        // Convert digits in the string to a list of integers
        List<int> values = valuesText
            .Where(char.IsDigit)
            .Select(character => character - '0')
            .ToList();

        return values;
    }

    private void SelectDice(FarkleTurnState turn, List<int> values)
    {
        // Add requested dice from the hand to selected dice
        foreach (int value in values)
        {
            int index = turn.HandDice.FindIndex(die => die.Value == value);
            turn.SelectedDice.Add(turn.HandDice[index]);
        }
    }

    private void RemoveSelectionFromHand(FarkleTurnState turn)
    {
        foreach (Die selectedDie in turn.SelectedDice)
        {
            // Find the index of a die with corresponding value
            int index = turn.HandDice.FindIndex(
                handDie => handDie.Value == selectedDie.Value);

            // Remove the corresponding die from the hand
            turn.HandDice.RemoveAt(index);
        }
    }
}
