// Albin Bagge
namespace DT_Console;

public class Game
{
    private List<Player> _players;
    private List<Opponent> _opponents;

    public Game()
    {
        _players = new List<Player>();
        _players.Add(new Player("Player 1"));

        _opponents = new List<Opponent>();
        _opponents.Add(new Opponent());

        Menu();
    }

    private void Menu()
    {
        bool menuActive = true;

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

        do
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Debug: case 1 Farkle");
                    break;

                case ConsoleKey.D2:
                    Console.WriteLine("Debug: case 2 Put & Take");
                    break;

                case ConsoleKey.D3:
                    Console.WriteLine("Debug: case 3 -");
                    break;

                case ConsoleKey.D4:
                    Console.WriteLine("Debug: case 4 -");
                    break;

                case ConsoleKey.D5:
                    Console.WriteLine("Debug: case 5 Choose players");
                    
                    // Todo: add players feature zzz
                    break;

                case ConsoleKey.D6:
                    Console.WriteLine("Debug: case 6 Choose AI opponents");
                    
                    // Todo: add opponents feature
                    break;

                case ConsoleKey.D7:
                    Console.WriteLine("Debug: case 7 Quit");

                    menuActive = false;
                    break;
            }
        }
        while (menuActive);
    }

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
}
