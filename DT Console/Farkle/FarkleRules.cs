// Albin Bagge
namespace DT_Console;

internal class FarkleRules
{
    // Checks if the given dice have a scoring combination
    internal bool HasScore(List<Die> dice)
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

    internal bool DiceContainsValues(List<Die> dice, List<int> values)
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

    // Add score from the best combinations of the given dice
    internal int ScoreDice(FarkleTurnState turn, List<Die> dice, bool printEnabled)
    {
        // Restore remaining dice
        turn.RemainingDice.Clear();
        turn.RemainingDice.AddRange(dice);

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

    // Print score, consume used dice & return score
    private int ScoreFullStraight(FarkleTurnState turn, bool printEnabled)
    {
        int comboScore = 1500;

        if (printEnabled)
        {
            // Print scoring dice
            Prints.PrintDiceList(turn.RemainingDice);
            Console.Write("= ");
            Prints.WriteInColor($"{comboScore}", ConsoleColor.Yellow);
            Console.Write(" score\n");
        }

        // Consume scoring dice
        turn.RemainingDice.Clear();

        return comboScore;
    }

    // Print score, consume used dice & return score
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
                Prints.PrintDieInColor(turn.RemainingDice[dieIndex], ConsoleColor.Yellow);
                Console.Write(" ");
            }
            Console.Write("= ");
            Prints.WriteInColor($"{comboScore}", ConsoleColor.Yellow);
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

    // Print score, consume used dice & return score
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
                Prints.PrintDieInColor(turn.RemainingDice[dieIndex], ConsoleColor.Yellow);
                Console.Write(" ");
            }
            Console.Write("= ");
            Prints.WriteInColor($"{comboScore}", ConsoleColor.Yellow);
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

    // Calculate score, print score & consume used dice
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
                        Prints.PrintDiceList(matchingDice);
                        Console.Write("= ");
                        Prints.WriteInColor($"{comboScore}", ConsoleColor.Yellow);
                        Console.Write(" score\n");
                    }

                    // Consume scoring dice
                    turn.RemainingDice.RemoveAll(die => die.Value == dieValue);
                }
            }
        }

        return score;
    }

    // Calculate score, print score & consume used dice
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
                Prints.PrintDieInColor(turn.RemainingDice[dieIndex], ConsoleColor.Yellow);
                Console.Write(" ");
            }
            Console.Write("= ");
            Prints.WriteInColor($"{comboScore}", ConsoleColor.Yellow);
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

    // Calculate score, print score & consume used dice
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
                Prints.PrintDiceList(onesDice);
                Console.Write("= ");
                Prints.WriteInColor($"{onesScore}", ConsoleColor.Yellow);
                Console.Write(" score\n");
            }
            if (fivesCount > 0)
            {
                // Print scoring dice
                Prints.PrintDiceList(fivesDice);
                Console.Write("= ");
                Prints.WriteInColor($"{fivesScore}", ConsoleColor.Yellow);
                Console.Write(" score\n");
            }
        }

        // Consume scoring dice
        turn.RemainingDice.RemoveAll(die => die.Value == 1 || die.Value == 5);

        return onesScore + fivesScore;
    }
}
