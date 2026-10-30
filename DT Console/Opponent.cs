// Albin Bagge
namespace DT_Console;

public class Opponent : Actor
{
    public Opponent(int index)
    {
        Name = PickName(index);
    }

    // Pick a name using an index 0-10
    private string PickName(int index)
    {
        List<string> names = [ 
            "Alfred", "Berta", "Charlie", "David", "Elias", 
            "Faith", "Goro", "Helena", "Isaac", "Juliette", 
            "Henry of Skalitz" ];

        return names[index];
    }
}
