// Albin Bagge
using System.Net;

namespace DT_Console;

public class Farkle
{
    private bool _gameModeActive;
    private List<Player> _players;
    private List<Opponent> _opponents;

    public Farkle(List<Player> players, List<Opponent> opponents)
    {
        _gameModeActive = true;
        _players = players;
        _opponents = opponents;
    }

    public void Start()
    {
        do
        {
            // Debug: Could also StartRound(_players) then StartRound(_opponents)
            StartRound();
            _gameModeActive = false; // Debug
            Utilities.WaitForKeyPress(); // Debug
        }
        while (_gameModeActive);
    }

    private void StartRound()
    {
        // Add Players & Opponents to one Actor list
        List<Actor> actors = new();
        actors.AddRange(_players);
        actors.AddRange(_opponents);

        foreach (Actor actor in actors)
        {
            StartTurn(actor);
        }
    }

    private void StartTurn(Actor actor)
    {
        bool turnActive = true;
        List<Die> remainingDice = new();
        List<Die> setAsideDice = new();

        // Add 6 new dice
        for (int i = 0; i < 6; i++)
        {
            remainingDice.Add(new Die());
        }

        // Loop turn
        do
        {
            // Throw remaining dice
            foreach (Die die in remainingDice)
            {
                die.Roll();
            }

            // Sort dice by value
            remainingDice = remainingDice.OrderBy(d => d.Value).ToList();

            // Print dice
            Console.Write($"{actor.Name} rolled:");
            foreach (Die die in remainingDice)
            {
                Console.Write($" {die.Value}");
            }
            Console.WriteLine();

            // If scoring combinations
            if (CheckIfScore(remainingDice))
            {
                Console.WriteLine(
                    $"1. Set aside\n" +
                    $"2. Stand");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("Debug: Set aside");
                        break;

                    case ConsoleKey.D2:
                        actor.GameScore = GetScore(remainingDice); // Debug
                        Console.WriteLine($"Debug - {actor.Name} game score: {actor.GameScore}");
                        break;
                }
                Console.WriteLine();

                // Set aside or stand
                // If all dice scored: ask to keep playing
            }
            else
            {
                turnActive = false;
                Console.WriteLine($"Debug: {actor.Name} farkled! (No scoring dice.)");
                Console.WriteLine();
            }

            turnActive = false; // Debug
        }
        while (turnActive);
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
