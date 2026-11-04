// Albin Bagge
using System.Collections;

namespace DT_Console;

internal class Farkle
{
    private readonly List<Player> _players;
    private readonly List<Opponent> _opponents;
    private readonly List<Actor> _actors = new();
    private const int WinScore = 1000; // Todo: change to 4000 in finished version

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
            // Throw remaining dice
            foreach (Die die in turn.RemainingDice)
            {
                die.Roll();
            }

            // Sort dice by value for better presentation
            turn.RemainingDice.Sort((first, second) => first.Value.CompareTo(second.Value));

            // Print dice
            Console.Write($"{turn.Actor.Name} rolled:   ");
            foreach (Die die in turn.RemainingDice)
            {
                Console.Write($" {die.Value}");
            }
            Console.WriteLine();

            // Check if the turn should continue
            if (CheckIfScore(turn.RemainingDice))
            {
                Console.WriteLine(
                    $"1. Set aside\n" +
                    $"2. Stand");

                // Choose: set aside / stand
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        {
                            Console.WriteLine("Debug: Set aside");

                            // Todo:
                            // Input string of dice values to set aside
                                // Convert string into a list of single-digit integers
                                // Check that the dice-list contains all values in the int-list
                                    // Add & remove dice corresponding to the int-list...
                                    // ...from remainingDice to setAsideDice
                                    // (foreach (nr in int-list) -> remove dice where value == nr)

                                    // Get score from setAsideDice and add to turn score
                                    // Empty list of set aside dice

                            break;
                        }

                    case ConsoleKey.D2:
                        // Add final turn score to actor's core
                        turn.Score += GetScore(turn.RemainingDice);
                        turn.Actor.Score += turn.Score;

                        // Print scores
                        Console.WriteLine($"{turn.Actor.Name} stands. " +
                            $"Turn score: {turn.Score}, Total score: {turn.Actor.Score}");

                        // End the turn
                        turn.IsActive = false;

                        break;
                }
                Console.WriteLine();

                // Check if all dice scored
                if (turn.RemainingDice.Count() == 0)
                {
                    Console.WriteLine($"All of {turn.Actor}'s dice scored! " +
                        $"Do they wish to keep playing their turn with new dice?\n" +
                        $"1. Yes\n" +
                        $"2. No");

                    // Choose: add dice / end turn
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.D1:
                            turn.AddDice();
                            break;

                        case ConsoleKey.D2:
                            turn.IsActive = false;
                            break;
                    }
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
    private int GetScore(List<Die> dice)
    {
        // Total score for the given dice
        int score = 0;

        // Count numbers of dice with the special values 1 & 5
        int ones = dice.Where(d => d.Value == 1).Count();
        int fives = dice.Where(d => d.Value == 5).Count();

        // Count the number of different values among the dice (max 6 for D6's)
        int distinctDice = dice
            .Select(d => d.Value)
            .Distinct()
            .Count();

        // Check for full straight
        if (distinctDice == 6)
        {
            score += 1500;
            ones--; // Remove used ones
            fives--; // Remove used fives
            Console.WriteLine($"Debug - full straight, score: {score}");
        }
        else if (distinctDice == 5)
        {
            // Check for small straight
            if (!dice.Any(d => d.Value == 6))
            {
                score += 500;
                ones--;
                fives--;
                Console.WriteLine($"Debug - small straight, score: {score}");
            }
            // Check for large straight
            else if (!dice.Any(d => d.Value == 1))
            {
                score += 750;
                fives--;
                Console.WriteLine($"Debug - large straight, score: {score}");
            }
        }
        // Check for matching dice
        else
        {
            // Create a list of groups of matching dice
            List<IGrouping<int, Die>> groups = dice
                .GroupBy(d => d.Value)
                .ToList();

            // For each group of matching dice
            foreach (IGrouping<int, Die> group in groups)
            {
                int nrOfMatchingDice = group.Count();
                int diceValue = group.Key;

                if (nrOfMatchingDice >= 3)
                {
                    // Handle triple ones for special score
                    if (diceValue == 1)
                    {
                        int nrOfTripleOnes = nrOfMatchingDice / 3;
                        int remainingOnes = nrOfMatchingDice % 3;

                        score += nrOfTripleOnes * 1000;
                        ones = remainingOnes; // Remove used ones
                        Console.WriteLine($"Debug - {nrOfTripleOnes} triple ones, score: {score}");
                    }
                    // Handle other matching dice values
                    else
                    {
                        // Score multiplier depending on the number of matching dice
                        int scoreMultiplier = (int)Math.Pow(2, nrOfMatchingDice - 3);

                        // Calculate & add score
                        score += diceValue * 100 * scoreMultiplier;
                        Console.WriteLine(
                            $"Debug - {nrOfMatchingDice}*{diceValue}, score: {score}");

                        // Remove used fives from further score calculation
                        if (diceValue == 5)
                        {
                            fives -= nrOfMatchingDice;
                        }
                    }
                }
            }
        }

        // Add score from remaining ones & fives
        score += ones * 100 + fives * 50;
        Console.WriteLine($"Debug - {ones} ones & {fives} fives, score: {score}");

        return score;
    }
}
