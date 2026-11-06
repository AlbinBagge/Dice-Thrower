// Albin Bagge
namespace DT_Console;

internal class Farkle
{
    private readonly List<Player> _players;
    private readonly List<Opponent> _opponents;
    private readonly List<Actor> _actors = new();
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

        PrintStartScreen();

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

            if (HasScore(turn.HandDice))
            {
                ChooseSetAsideOrStand(turn);

                // Update the number of non-scoring dice remaining in the hand
                GetScore(turn, turn.HandDice, false);

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

    // Checks if the given dice have a scoring combination
    private bool HasScore(List<Die> dice)
    {
        bool rollScored = false;

        // Check for ones
        if (dice.Any(d => d.Value == 1))
        {
            rollScored = true;
        }
        // Check for fives
        else if (dice.Any(d => d.Value == 5))
        {
            rollScored = true;
        }
        // Check for three of a kind
        else if (dice.GroupBy(die => die.Value).Any(group => group.Count() >= 3))
        {
            rollScored = true;
        }

        return rollScored;
    }

    // Gets the score from the best combination of the given dice
    private int GetScore(FarkleTurnState turn, List<Die> dice, bool printEnabled)
    {
        PrepareRemainingDice(turn, dice);

        // Total score for the given list of dice
        int score = 0;

        // Count the number of dice with distinct values
        int nrOfDistinctDice = dice
            .Select(d => d.Value)
            .Distinct()
            .Count();

        // Check for straights
        if (nrOfDistinctDice == Die.NrOfSides)
        {
            score += ScoreFullStraight(turn, printEnabled);
        }
        else if (nrOfDistinctDice == (Die.NrOfSides - 1))
        {
            if (!dice.Any(d => d.Value == Die.NrOfSides))
            {
                score += ScoreSmallStraight(turn, printEnabled);
            }
            else if (!dice.Any(d => d.Value == 1))
            {
                score += ScoreLargeStraight(turn, printEnabled);
            }
        }
        // Check for matching dice
        else
        {
            score += ScoreMatchingDice(turn, dice, printEnabled);
        }
        
        score += ScoreOnesAndFives(turn, printEnabled);

        return score;
    }

    private bool DiceContainsValues(List<Die> dice, List<int> values)
    {
        // Create a sequence of groups of matching values
        IEnumerable<IGrouping<int, int>> valueGroups = values.GroupBy(value => value);

        foreach (IGrouping<int, int> group in valueGroups)
        {
            int value = group.Key;
            int valueCount = group.Count();

            // Number of dice with the requested value
            int diceCount = dice.Count(die => die.Value == value);

            // Check if there are enough dice with the requested value 
            if (diceCount < valueCount)
            {
                return false;
            }
        }

        return true;
    }

    private void PrintStartScreen()
    {
        Console.Clear();

        // Title
        Console.WriteLine("> Dice Thrower > Farkle");
        Console.WriteLine();

        // List of Players & Opponents
        Utilities.PrintActors(_players, _opponents);
        Console.WriteLine();
    }

    private void PrintDieInColor(Die die, ConsoleColor backgroundColor)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = backgroundColor;
        Console.Write($" {die.Value} ");
        Console.ResetColor();
    }

    private void PrintDiceList(List<Die> dice)
    {
        foreach (Die die in dice)
        {
            PrintDieInColor(die, ConsoleColor.Yellow);
            Console.Write(" ");
        }
    }

    private void WriteInColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
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
        PrintDiceList(turn.HandDice);
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
        WriteInColor($"All of {turn.Actor.Name}'s dice scored!\n", ConsoleColor.Green);
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
            "SET ASIDE - Input values to set aside, with or without spaces, " +
            "and press Enter:");

        bool isSettingAside = true;
        do
        {
            // Reset selected dice & their score
            turn.SelectedDice.Clear();
            turn.TempScore = 0;

            // Input string for dice selection
            string valuesText;
            do
            {
                valuesText = Console.ReadLine() ?? string.Empty;
            }
            while (string.IsNullOrWhiteSpace(valuesText));

            // Convert digits in the string to a list of integers
            List<int> valuesList = valuesText
                .Where(char.IsDigit)
                .Select(character => character - '0')
                .ToList();

            // Make sure the list of requested values isn't empty
            if (valuesList.Count == 0)
            {
                continue;
            }

            // Make sure the hand contains all requested values
            if (!DiceContainsValues(turn.HandDice, valuesList))
            {
                continue;
            }

            // Add requested dice from the hand to selected dice
            foreach (int value in valuesList)
            {
                int index = turn.HandDice.FindIndex(die => die.Value == value);
                turn.SelectedDice.Add(turn.HandDice[index]);
            }

            // Obtain the score from selected dice
            turn.TempScore = GetScore(turn, turn.SelectedDice, true);

            // Check if there are non-scoring dice remaining from selected dice
            // (Non-scoring dice can not be set aside)
            if (turn.RemainingDice.Count() != 0)
            {
                continue;
            }

            // Empty line after successfull input
            Console.WriteLine();

            // Add score from dice selected to be set aside
            turn.Score += turn.TempScore;

            // Set aside selected dice
            foreach (Die selectedDie in turn.SelectedDice)
            {
                // Find the index of a die with corresponding value
                int index = turn.HandDice.FindIndex(
                    handDie => handDie.Value == selectedDie.Value);

                // Remove the corresponding die from the hand
                turn.HandDice.RemoveAt(index);
            }

            isSettingAside = false;
        }
        while (isSettingAside);
    }

    private void Stand(FarkleTurnState turn)
    {
        // Print choice
        Console.WriteLine("STAND");

        // Add final turn score to actor's core
        turn.Score += GetScore(turn, turn.HandDice, true);
        turn.Actor.Score += turn.Score;

        // Print scores
        Console.Write($"{turn.Actor.Name} stands. (TURN: ");
        WriteInColor($"{turn.Score}", ConsoleColor.Yellow);
        Console.Write($" score / TOTAL: ");
        WriteInColor($"{turn.Actor.Score}", ConsoleColor.Yellow);
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
            WriteInColor($"The winner is {winner.Name} with {winner.Score} score!\n",
                ConsoleColor.Green);
            Console.WriteLine();
            Utilities.WaitForKeyPress();
        }
    }

    private void EndTurnWithFarkle(FarkleTurnState turn)
    {
        // Farkle (end turn without score)
        turn.IsActive = false;
        WriteInColor($"{turn.Actor.Name} farkled! (No score.)\n", ConsoleColor.Red);
        Console.WriteLine();
    }

    private void PrepareRemainingDice(FarkleTurnState turn, List<Die> dice)
    {
        turn.RemainingDice.Clear();
        turn.RemainingDice.AddRange(dice);
    }

    private int ScoreFullStraight(FarkleTurnState turn, bool printEnabled)
    {
        int comboScore = 1500;

        if (printEnabled)
        {
            // Print scoring dice
            PrintDiceList(turn.RemainingDice);
            Console.Write("= ");
            WriteInColor($"{comboScore}", ConsoleColor.Yellow);
            Console.Write(" score\n");
        }

        // Consume scoring dice
        turn.RemainingDice.Clear();

        return comboScore;
    }

    private int ScoreSmallStraight(FarkleTurnState turn, bool printEnabled)
    {
        int comboScore = 500;

        if (printEnabled)
        {
            // Print scoring dice
            for (int i = 0; i < (Die.NrOfSides - 1); i++)
            {
                int dieValue = i + 1;
                int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == dieValue);
                PrintDieInColor(turn.RemainingDice[dieIndex], ConsoleColor.Yellow);
                Console.Write(" ");
            }
            Console.Write("= ");
            WriteInColor($"{comboScore}", ConsoleColor.Yellow);
            Console.Write(" score\n");
        }

        // Consume scoring dice
        for (int i = 0; i < (Die.NrOfSides - 1); i++)
        {
            int dieValue = i + 1;
            int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == dieValue);
            turn.RemainingDice.RemoveAt(dieIndex);
        }

        return comboScore;
    }

    private int ScoreLargeStraight(FarkleTurnState turn, bool printEnabled)
    {
        int comboScore = 750;

        if (printEnabled)
        {
            // Print scoring dice
            for (int i = 1; i < Die.NrOfSides; i++)
            {
                int dieValue = i + 1;
                int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == dieValue);
                PrintDieInColor(turn.RemainingDice[dieIndex], ConsoleColor.Yellow);
                Console.Write(" ");
            }
            Console.Write("= ");
            WriteInColor($"{comboScore}", ConsoleColor.Yellow);
            Console.Write(" score\n");
        }

        // Consume scoring dice
        for (int i = 1; i < Die.NrOfSides; i++)
        {
            int dieValue = i + 1;
            int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == dieValue);
            turn.RemainingDice.RemoveAt(dieIndex);
        }

        return comboScore;
    }

    private int ScoreMatchingDice(FarkleTurnState turn, List<Die> dice, bool printEnabled)
    {
        int score = 0;

        // Create a sequence of groups of matching dice
        IEnumerable<IGrouping<int, Die>> groups = dice.GroupBy(d => d.Value);

        foreach (IGrouping<int, Die> group in groups)
        {
            int dieValue = group.Key;
            int matchingDiceCount = group.Count();

            if (matchingDiceCount >= 3)
            {
                // Check for three ones (special score)
                if (dieValue == 1)
                {
                    score += ScoreThreeOnes(turn, printEnabled, matchingDiceCount);
                }
                // Other matching dice values
                else
                {
                    // Score multiplier depending on the number of matching dice
                    int scoreMultiplier = (int)Math.Pow(2, matchingDiceCount - 3);

                    // Calculate score for the combination
                    int comboScore = dieValue * 100 * scoreMultiplier;

                    score += comboScore;

                    if (printEnabled)
                    {
                        // Print scoring dice
                        List<Die> matchingDice = new();
                        matchingDice.AddRange(turn.RemainingDice.Where(die => die.Value == dieValue));
                        PrintDiceList(matchingDice);
                        Console.Write("= ");
                        WriteInColor($"{comboScore}", ConsoleColor.Yellow);
                        Console.Write(" score\n");
                    }

                    // Consume scoring dice
                    turn.RemainingDice.RemoveAll(die => die.Value == dieValue);
                }
            }
        }

        return score;
    }

    private int ScoreThreeOnes(FarkleTurnState turn, bool printEnabled, int onesCount)
    {
        int tripleOnesCount = onesCount / 3;
        int remainingOnes = onesCount % 3;
        int scoringOnes = onesCount - remainingOnes;
        int comboScore = tripleOnesCount * 1000;

        if (printEnabled)
        {
            // Print scoring dice
            for (int i = 0; i < scoringOnes; i++)
            {
                int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == 1);
                PrintDieInColor(turn.RemainingDice[dieIndex], ConsoleColor.Yellow);
                Console.Write(" ");
            }
            Console.Write("= ");
            WriteInColor($"{comboScore}", ConsoleColor.Yellow);
            Console.Write(" score\n");
        }

        // Consume scoring dice
        for (int i = 0; i < scoringOnes; i++)
        {
            int dieIndex =
                turn.RemainingDice.FindIndex(die => die.Value == 1);
            turn.RemainingDice.RemoveAt(dieIndex);
        }

        return comboScore;
    }

    private int ScoreOnesAndFives(FarkleTurnState turn, bool printEnabled)
    {
        // Count number of remaining ones & fives
        List<Die> onesDice = new();
        onesDice.AddRange(turn.RemainingDice.Where(die => die.Value == 1));
        int onesCount = onesDice.Count();

        List<Die> fivesDice = new();
        fivesDice.AddRange(turn.RemainingDice.Where(die => die.Value == 5));
        int fivesCount = fivesDice.Count();

        // Calculate score from remaining ones & fives
        int onesScore = onesCount * 100;
        int fivesScore = fivesCount * 50;

        if (printEnabled)
        {
            if (onesCount > 0)
            {
                // Print scoring dice
                PrintDiceList(onesDice);
                Console.Write("= ");
                WriteInColor($"{onesScore}", ConsoleColor.Yellow);
                Console.Write(" score\n");
            }
            if (fivesCount > 0)
            {
                // Print scoring dice
                PrintDiceList(fivesDice);
                Console.Write("= ");
                WriteInColor($"{fivesScore}", ConsoleColor.Yellow);
                Console.Write(" score\n");
            }
        }

        // Consume scoring dice
        turn.RemainingDice.RemoveAll(die => die.Value == 1 || die.Value == 5);

        return onesScore + fivesScore;
    }
}
