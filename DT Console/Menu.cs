// Albin Bagge
namespace DT_Console;

internal class Menu
{
    private readonly List<Player> _players = new();
    private readonly List<Opponent> _opponents = new();
    private const int _maxActors = 10;

    internal Menu()
    {
        _players.Add(new Player("Player 1"));
        _players.Add(new Player("Player 2"));
    }

    internal void Start()
    {
        bool menuIsActive = true;

        // Menu loop
        do
        {
            PrintMenu();

            // Choose: a game / a setting / quit
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Farkle farkle = new(_players, _opponents);
                    farkle.StartGame();
                    break;

                case ConsoleKey.D2:
                    ChoosePlayers();
                    break;

                case ConsoleKey.D3:
                    ChooseOpponents();
                    break;

                case ConsoleKey.D4:
                    PrintRules();
                    break;

                case ConsoleKey.D5:
                    menuIsActive = false; // Quit
                    break;
            }
        }
        while (menuIsActive);
    }

    // Print main menu
    private void PrintMenu()
    {
        Console.Clear();

        // Title
        Console.WriteLine("> Dice Thrower");
        Console.WriteLine();

        // List of players & opponents
        Prints.PrintActors(_players, _opponents);
        Console.WriteLine();

        // Options
        Console.WriteLine(
            "Choose an option by pressing a number key:\n" +
            "1. Play Farkle\n" +
            "2. Choose players\n" +
            "3. Choose AI opponents\n" +
            "4. Rules\n" +
            "5. Quit\n");
    }

    private void PrintRules()
    {
        Console.Clear();

        // Title
        Console.WriteLine("> Dice Thrower > Rules");
        Console.WriteLine();

        // Rules text
        Console.WriteLine(
            "FARKLE\n" +
            "\n" +
            "Try to roll the best scoring combinations.\n" +
            "\n" +
            "Combination scores:\n" +
            "- Full straight  (1-6)    1500\n" +
            "- Large straight (2-6)     750\n" +
            "- Small straight (1-5)     500\n" +
            "- Three ones              1000\n" +
            "- Single one               100\n" +
            "- Single five               50\n" +
            "- Three of a kind         Value * 100\n" +
            "- Four of a kind          Value * 100 * 2\n" +
            "- Five of a kind          Value * 100 * 4\n" +
            "- six of a kind           Value * 100 * 8\n" +
            "\n" +
            "After rolling, choose if you want to set aside dice.\n" +
            "Then keep rolling the remaining dice.\n" +
            "\n" +
            "If you at any point don't roll any scoring dice you lose the score from that turn.\n" +
            "The more dice you set aside, the more risk you take.\n" +
            "\n" +
            "The player who first reaches 4000 score wins.\n");

        Prints.WaitForKeyPress();
    }

    // Empty the list of players & add new players
    private void ChoosePlayers()
    {
        string playerName;
        int numberOfPlayers;
        int numberOfOpponents = _opponents.Count();
        int leftOverOpponents;
        int exceedingOpponents;

        // Print choice
        Console.WriteLine("CHOOSE PLAYERS");
        Console.WriteLine();

        // Input new number of players
        do
        {
            numberOfPlayers = InputNumber($"Input number of players (1-{_maxActors}): ");
        }
        while (numberOfPlayers < 1 || numberOfPlayers > _maxActors);

        // Empty the list of players
        _players.Clear();

        // Add new players to the list
        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerName = InputName($"Enter a unique name for Player {i + 1}: ");
            _players.Add(new Player(playerName));
        }

        // Remove any opponents that exceed the maximum number of actors
        if ((numberOfPlayers + numberOfOpponents) > _maxActors)
        {
            leftOverOpponents = _maxActors - numberOfPlayers;
            exceedingOpponents = numberOfPlayers + numberOfOpponents - _maxActors;
            _opponents.RemoveRange(leftOverOpponents, exceedingOpponents);
        }
        // Ensure that a single player has an opponent
        else if (numberOfPlayers == 1 && numberOfOpponents == 0)
        {
            _opponents.Add(new Opponent(10));
        }
    }

    // Empty the list of opponents & add new opponents
    private void ChooseOpponents()
    {
        int numberOfPlayers = _players.Count();
        int numberOfOpponents;
        int minOpponents = 0;
        int maxOpponents = _maxActors - numberOfPlayers;

        // Print choice
        Console.WriteLine("CHOOSE OPPONENTS");
        Console.WriteLine();

        // Check if opponents can be added
        if (maxOpponents == 0)
        {
            Console.WriteLine("Max number of players reached. Remove players to add AI opponents.");
            Prints.WaitForKeyPress();
            return;
        }

        // Ensure that a single player has an opponent
        if (numberOfPlayers == 1)
        {
            minOpponents = 1;
        }

        // Input new number of opponents
        do
        {
            numberOfOpponents = InputNumber(
                $"Input number of opponents ({minOpponents}-{maxOpponents}): ");
        }
        while (numberOfOpponents < minOpponents || numberOfOpponents > maxOpponents);

        // Empty the list of opponents
        _opponents.Clear();

        // Add new opponents to the list
        for (int i = 0; i < numberOfOpponents; i++)
        {
            _opponents.Add(new Opponent(i));
        }
    }

    // Ask the user for a number
    private int InputNumber(string prompt)
    {
        bool success;
        int number;

        do
        {
            // Prompt the user for a number
            Console.Write(prompt);

            // Input number
            success = int.TryParse(Console.ReadLine(), out number);
        }
        while (!success);

        return number;
    }

    // Ask the user for a name & check for name conflicts
    private string InputName(string prompt)
    {
        string playerName;
        bool badName;

        do
        {
            // Prompt the user for a name
            Console.Write(prompt);

            // Input name
            playerName = Console.ReadLine() ?? string.Empty;

            // Check for bad name
            if (string.IsNullOrWhiteSpace(playerName))
            {
                badName = true;
            }
            else if (_players.Any(p => p.Name == playerName))
            {
                badName = true;
            }
            else if (_opponents.Any(o => o.Name == playerName))
            {
                badName = true;
            }
            else
            {
                badName = false;
            }
        }
        while (badName);

        return playerName;
    }
}
