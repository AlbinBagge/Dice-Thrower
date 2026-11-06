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
                // Print winner
                Actor? winner = _actors.MaxBy(actor => actor.Score);
                if (winner != null)
                {
                    Console.WriteLine($"The winner is {winner.Name} with {winner.Score} score!");
                    Utilities.WaitForKeyPress();
                }

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

        // Turn loop
        do
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

            // Check if the turn should continue
            if (CheckIfScore(turn.HandDice))
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
                        // Set aside
                        case ConsoleKey.D1:
                            Console.WriteLine(
                                "SET ASIDE - Input values to set aside, with or without spaces:");

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
                                turn.TempScore = GetScore(turn.SelectedDice, turn);

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

                            menuIsActive = false;
                            break;

                        // Stand
                        case ConsoleKey.D2:
                            // Add final turn score to actor's core
                            turn.Score += GetScore(turn.HandDice, turn);
                            turn.Actor.Score += turn.Score;

                            // Print scores
                            Console.WriteLine($"{turn.Actor.Name} stands. " +
                                $"Turn score: {turn.Score}, Total score: {turn.Actor.Score}");
                            Console.WriteLine();

                            // End the turn
                            turn.IsActive = false;
                            menuIsActive = false;
                            break;
                    }
                }
                while (menuIsActive);


                // Check if all dice scored
                if (turn.HandDice.Count() == 0)
                {
                    Console.WriteLine($"All of {turn.Actor}'s dice scored! " +
                        $"Do they wish to keep playing their turn with new dice?\n" +
                        $"1. Yes\n" +
                        $"2. No\n");

                    menuIsActive = true;
                    do
                    {
                        // Choose: add dice / end turn
                        switch (Console.ReadKey(true).Key)
                        {
                            case ConsoleKey.D1:
                                turn.AddDice();
                                menuIsActive = false;
                                break;

                            case ConsoleKey.D2:
                                turn.IsActive = false;
                                menuIsActive = false;
                                break;
                        }
                    }
                    while (menuIsActive);
                }
            }
            else
            {
                // Farkle (bust)
                turn.IsActive = false;
                Console.WriteLine($"{turn.Actor.Name} farkled! (No scoring dice.)");
                Console.WriteLine();
            }
        }
        while (turn.IsActive);
    }

    // Checks if the given dice have a scoring combination
    private bool CheckIfScore(List<Die> dice)
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
    private int GetScore(List<Die> dice, FarkleTurnState turn)
    {
        // Reset remaining dice
        turn.RemainingDice.Clear();
        turn.RemainingDice.AddRange(dice);

        // Total score for the given list of dice
        int score = 0;

        // Count the number of dice with distinct values
        int nrOfDistinctDice = dice
            .Select(d => d.Value)
            .Distinct()
            .Count();

        // Check for full straight
        if (nrOfDistinctDice == Die.NrOfSides)
        {
            int comboScore = 1500;
            score += comboScore;

            // Print scoring dice
            PrintDiceList(turn.RemainingDice);
            Console.Write($"= {comboScore} score");
            Console.WriteLine();

            // Consume scoring dice
            turn.RemainingDice.Clear();
        }
        else if (nrOfDistinctDice == (Die.NrOfSides - 1))
        {
            // Check for small straight
            if (!dice.Any(d => d.Value == Die.NrOfSides))
            {
                int comboScore = 500;
                score += comboScore;

                // Print & consume scoring dice
                for (int i = 0; i < (Die.NrOfSides - 1); i++)
                {
                    int dieValue = i + 1;
                    int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == dieValue);
                    PrintDie(turn.RemainingDice[dieIndex]); // Print die
                    turn.RemainingDice.RemoveAt(dieIndex); // Consume die
                }
                Console.Write($" = {comboScore} score");
                Console.WriteLine();
            }
            // Check for large straight
            else if (!dice.Any(d => d.Value == 1))
            {
                int comboScore = 750;
                score += comboScore;

                // Print & consume scoring dice
                for (int i = 1; i < Die.NrOfSides; i++)
                {
                    int dieValue = i + 1;
                    int dieIndex = turn.RemainingDice.FindIndex(die => die.Value == dieValue);
                    PrintDie(turn.RemainingDice[dieIndex]); // Print die
                    turn.RemainingDice.RemoveAt(dieIndex); // Consume die
                }
                Console.Write($" = {comboScore} score");
                Console.WriteLine();
            }
        }
        // Check for matching dice
        else
        {
            // Create a sequence of groups of matching dice
            IEnumerable<IGrouping<int, Die>> groups = dice.GroupBy(d => d.Value);

            foreach (IGrouping<int, Die> group in groups)
            {
                int dieValue = group.Key;
                int matchingDiceCount = group.Count();

                if (matchingDiceCount >= 3)
                {
                    // Handle special score for 3 ones
                    if (dieValue == 1)
                    {
                        int tripleOnesCount = matchingDiceCount / 3;
                        int remainingOnes = matchingDiceCount % 3;
                        int scoringOnes = matchingDiceCount - remainingOnes;
                        int comboScore = tripleOnesCount * 1000;

                        score += comboScore;

                        //Console.WriteLine($"{scoringOnes}*{dieValue}: {comboScore} score");

                        // Print scoring dice
                        for (int i = 0; i < scoringOnes; i++)
                        {
                            int dieIndex =
                                turn.RemainingDice.FindIndex(die => die.Value == dieValue);
                            PrintDie(turn.RemainingDice[dieIndex]);
                        }
                        Console.Write($" = {comboScore} score");
                        Console.WriteLine();

                        // Consume scoring dice
                        for (int i = 0; i < scoringOnes; i++)
                        {
                            int dieIndex = 
                                turn.RemainingDice.FindIndex(die => die.Value == dieValue);
                            turn.RemainingDice.RemoveAt(dieIndex);
                        }
                    }
                    // Handle score for other matching dice values
                    else
                    {
                        // Score multiplier depending on the number of matching dice
                        int scoreMultiplier = (int)Math.Pow(2, matchingDiceCount - 3);

                        // Calculate score for the combination
                        int comboScore = dieValue * 100 * scoreMultiplier;

                        score += comboScore;

                        // Print scoring dice
                        List<Die> matchingDice = new();
                        matchingDice.AddRange(turn.RemainingDice.Where(die => die.Value == dieValue));
                        PrintDiceList(matchingDice);
                        Console.Write($"= {comboScore} score");
                        Console.WriteLine();

                        // Consume scoring dice
                        turn.RemainingDice.RemoveAll(die => die.Value == dieValue);
                    }
                }
            }
        }

        // Count remaining ones & fives
        List<Die> matchDice = new();
        matchDice.AddRange(turn.RemainingDice.Where(die => die.Value == 1)); // Simplify assignment?
        int onesCount = matchDice.Count(); // Use count property?

        List<Die> fivesDice = new();
        fivesDice.AddRange(turn.RemainingDice.Where(die => die.Value == 5)); // Simplify assignment?
        int fivesCount = fivesDice.Count(); // Use count property?

        // Calculate score from remaining ones & fives
        int onesScore = onesCount * 100;
        int fivesScore = fivesCount * 50;

        score += onesScore + fivesScore;

        if (onesCount > 0)
        {
            PrintDiceList(matchDice);
            Console.Write($"= {onesScore} score");
            Console.WriteLine();
        }
        if (fivesCount > 0)
        {
            PrintDiceList(fivesDice);
            Console.Write($"= {fivesScore} score");
            Console.WriteLine();
        }

        // Consume scoring dice
        turn.RemainingDice.RemoveAll(die => die.Value == 1 || die.Value == 5);

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

    private void PrintDie(Die die)
    {
        // Add colors
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.Write($" {die.Value} ");
        Console.ResetColor();
    }

    // Remove old method?:
    /*
    private void PrintDieValue(int dieValue)
    {
        // Add colors
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.Write($" {dieValue} ");
        Console.ResetColor();
    }*/

    private void PrintDiceList(List<Die> dice)
    {
        foreach (Die die in dice)
        {
            //PrintDieValue(die.Value); // Remove?
            PrintDie(die);
            Console.Write(" ");
        }
    }
}
