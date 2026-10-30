// Albin Bagge
namespace DT_Console;

public class Game
{
    private int _maxActors;
    private List<Player> _players;
    private List<Opponent> _opponents;

    public Game()
    {
        _maxActors = 10;

        _players = new List<Player>();
        _players.Add(new Player("Player 1"));

        _opponents = new List<Opponent>();
        _opponents.Add(new Opponent(0));
    }

    public void StartMenu()
    {
        bool menuActive = true;

        do
        {
            Console.Clear();

            // Title
            Console.WriteLine("> Dice Thrower");
            Console.WriteLine();

            // List of Players & Opponents
            PrintActors();
            Console.WriteLine();

            // Options
            Console.WriteLine(
                "Choose an option by pressing a number key:\n" +
                "1. Play Farkle\n" +
                "2. Play Put & Take\n" +
                "3. -\n" +
                "4. -\n" +
                "5. Choose players\n" +
                "6. Choose AI opponents\n" +
                "7. Quit\n");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Debug: case 1 Farkle");
                    WaitForKeyPress();
                    // Todo: Farkle.cs

                    break;

                case ConsoleKey.D2:
                    Console.WriteLine("Debug: case 2 Put & Take");
                    WaitForKeyPress();
                    // Todo: PutAndtake.cs

                    break;

                case ConsoleKey.D3:
                    Console.WriteLine("Debug: case 3 -");
                    WaitForKeyPress();
                    // Todo (bonus): FiveNine.cs

                    break;

                case ConsoleKey.D4:
                    Console.WriteLine("Debug: case 4 -");
                    WaitForKeyPress();
                    // Todo (bonus): Beetle.cs

                    break;

                case ConsoleKey.D5:
                    ChoosePlayers();
                    break;

                case ConsoleKey.D6:
                    ChooseOpponents();
                    break;

                case ConsoleKey.D7:
                    menuActive = false;
                    break;
            }
        }
        while (menuActive);
    }

    private void WaitForKeyPress()
    {
        Console.Write("Press any key: ");
        Console.ReadKey();
    }

    // Print current players & opponents
    private void PrintActors()
    {
        Console.WriteLine("Current players:");
        foreach (Player player in _players)
        {
            Console.WriteLine($"- {player.Name}");
        }
        Console.WriteLine();

        Console.WriteLine("Current AI opponents:");
        foreach (Opponent opponent in _opponents)
        {
            Console.WriteLine($"- {opponent.Name}");
        }
    }

    // Empty the list of Players & add new Players
    private void ChoosePlayers()
    {
        string playerName;
        int numberOfPlayers;
        int numberOfOpponents = _opponents.Count();
        int leftOverOpponents;
        int exceedingOpponents;

        // Input new number of Players
        do
        {
            numberOfPlayers = InputNumber($"Input number of players (1-{_maxActors}): ");
        }
        while (numberOfPlayers < 1 || numberOfPlayers > _maxActors);

        // Empty the list of Players
        _players.Clear();

        // Add new Players to the list
        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerName = InputName($"Enter a unique name for Player {i + 1}: ");
            _players.Add(new Player(playerName));
        }

        // Remove any Opponents that exceed the maximum number of Actors
        if ((numberOfPlayers + numberOfOpponents) > _maxActors)
        {
            leftOverOpponents = _maxActors - numberOfPlayers;
            exceedingOpponents = numberOfPlayers + numberOfOpponents - _maxActors;
            _opponents.RemoveRange(leftOverOpponents, exceedingOpponents);
        }
        // Ensure that a single Player has an Opponent
        else if (numberOfPlayers == 1 && numberOfOpponents == 0)
        {
            _opponents.Add(new Opponent(10));
        }
    }

    // Empty the list of Opponents & add new Opponents
    private void ChooseOpponents()
    {
        int numberOfPlayers = _players.Count();
        int numberOfOpponents;
        int minOpponents = 0;
        int maxOpponents = _maxActors - numberOfPlayers;

        // Check if Opponents can be added
        if (maxOpponents == 0)
        {
            Console.WriteLine("Max number of players reached. Remove players to add AI opponents.");
            WaitForKeyPress();
            return;
        }

        // Ensure that a single Player has an Opponent
        if (numberOfPlayers == 1)
        {
            minOpponents = 1;
        }

        // Input new number of Opponents
        do
        {
            numberOfOpponents = InputNumber(
                $"Input number of opponents ({minOpponents}-{maxOpponents}): ");
        }
        while (numberOfOpponents < minOpponents || numberOfOpponents > maxOpponents);

        // Empty the list of Opponents
        _opponents.Clear();

        // Add new Opponents to the list
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
