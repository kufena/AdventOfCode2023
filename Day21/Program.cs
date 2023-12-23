// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int steps = int.Parse(args[1]);
Part1(lines, steps);

void Part1(string[] lines, int steps)
{
    (int, int)[] directions = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
    int xext = lines[0].Length;
    int yext = lines.Length;

    int startx = 0;
    int starty = 0;
    bool[][] grid = new bool[yext][];
    for (int i = 0; i < yext; i++) {
        grid[i] = new bool[xext];
        for (int j = 0; j < xext; j++)
        {
            grid[i][j] = (lines[i][j] == '.' || lines[i][j] == 'S');
            if (lines[i][j] == 'S')
            {
                startx = j;
                starty = i;
            }
        }
    }

    HashSet<(int,int)> gardens = new HashSet<(int,int)> ();
    gardens.Add((starty, startx));
    for (int step = 0; step < steps; step++)
    {
        HashSet<(int,int)> newgardens = new HashSet<(int,int)> ();
        foreach ((int gy, int gx) in gardens)
        {
            foreach ((int dy, int dx) in directions)
            {
                int py = gy + dy;
                int px = gx + dx;
                if (py >= 0 && py < yext && px >= 0 && px < xext && grid[py][px])
                {
                    newgardens.Add((py, px));
                }
            }
        }
        gardens = newgardens;
    }

    Console.WriteLine(gardens.Count);
}