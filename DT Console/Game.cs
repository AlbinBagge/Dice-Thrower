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
        _opponents.Add(new Opponent());
    }

    public void StartMenu()
    {
        bool menuActive = true;

        do
        {
            Console.Clear();
            Console.WriteLine("> Dice Thrower");
            Console.WriteLine();

            PrintActors();
            Console.WriteLine();

            Console.WriteLine(
                "Choose an option by pressing a number key:\n" +
                "1. Play Farkle\n" +
                "2. Play Put & Take\n" +
                "3. -\n" +
                "4. -\n" +
                "5. Choose players\n" +
                "6. Choose AI opponents\n" +
                "7. Quit");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Debug: case 1 Farkle");
                    WaitForKeyPress();
                    break;

                case ConsoleKey.D2:
                    Console.WriteLine("Debug: case 2 Put & Take");
                    WaitForKeyPress();
                    break;

                case ConsoleKey.D3:
                    Console.WriteLine("Debug: case 3 -");
                    WaitForKeyPress();
                    break;

                case ConsoleKey.D4:
                    Console.WriteLine("Debug: case 4 -");
                    WaitForKeyPress();
                    break;

                case ConsoleKey.D5:
                    ChoosePlayers();
                    WaitForKeyPress();
                    break;

                case ConsoleKey.D6:
                    ChooseOpponents();
                    WaitForKeyPress();
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
        Console.WriteLine("Press any key.");
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

    // Empty the list of players & add new players
    private void ChoosePlayers()
    {
        int numberOfPlayers;
        string playerName;

        // Input number of players
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
    }

    // Empty the list of opponents & add new opponents
    private void ChooseOpponents()
    {
        Console.WriteLine("Debug: ChooseOpponents");
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
