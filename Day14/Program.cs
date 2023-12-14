// See https://aka.ms/new-console-template for more information
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
// assume a square i guess.
int columns = lines[0].Length;
int rows = lines.Length;
char[][] grid = new char[rows][];
for (int j = 0; j < rows; j++)
{
    grid[j] = new char[columns];
    for (int k = 0; k < columns; k++)
    {
        grid[j][k] = lines[j][k];
    }
}

//Part1(grid);
Part2(grid);

static void Part1(char[][] lines)
{
    // assume a square i guess.
    int columns = lines[0].Length;
    int rows = lines.Length;
    int total;
    char[][] newgrid;
    NorthShuffle(lines, columns, rows, out total, out newgrid);

    for (int j = 0; j < rows; j++)
    {
        Console.WriteLine(new String(newgrid[j]));
    }
    Console.WriteLine($"Total = {total}");
}

static void Part2(char[][] lines)
{
    HashSet<string> cache = new();
    var initstr = GridToString(lines);
    cache.Add(initstr);
    Dictionary<string, int> pointsOfOccurrence = new();
    pointsOfOccurrence.Add(initstr, 0);
    Dictionary<string, int> targetsOfOccurrence = new();
    targetsOfOccurrence.Add(initstr, 0);
    Dictionary<long, string> atPoints = new();
    atPoints.Add(0, initstr);

    for (int counter = 1; counter < 1000000001; counter++)
    {
        Console.WriteLine($"Cycle {counter}");
        // assume a square i guess.
        int columns = lines[0].Length;
        int rows = lines.Length;
        int total;
        char[][] newgrid;
        NorthShuffle(lines, columns, rows, out total, out newgrid);
        lines = newgrid;
        WestShuffle(lines, columns, rows, out total, out newgrid);
        lines = newgrid;
        SouthShuffle(lines, columns, rows, out total, out newgrid);
        lines = newgrid;
        EastShuffle(lines, columns, rows, out total, out newgrid);
        
        var cachestr = GridToString(newgrid);
        if (cache.Contains(cachestr))
        {
            int atPoint = pointsOfOccurrence[cachestr];
            int cycle = counter - atPoint;
            int thepoint = atPoint + ((1000000000 - atPoint) % cycle);
            total = targetsOfOccurrence[ atPoints[thepoint] ];

            Console.WriteLine($"Cache hit after {counter} spins! Previous at {atPoint}.  Score = {total}");
            break;
        }
        total = Score(newgrid);
        cache.Add(cachestr);
        pointsOfOccurrence.Add(cachestr, counter);
        targetsOfOccurrence.Add(cachestr, total);
        atPoints.Add(counter, cachestr);

        for (int j = 0; j < rows; j++)
        {
            Console.WriteLine(new String(newgrid[j]));
        }
        Console.WriteLine($"Total = {total}");
        
        lines = newgrid;
    }
}


static string GridToString(char[][] grid)
{
    StringBuilder sb = new();
    for (int i = 0; i < grid.Length; i++)
    {
        for (int j = 0; j < grid[i].Length; j++)
        {
            sb.Append(grid[i][j]);
        }
    }
    return sb.ToString();
}

static void NorthShuffle(char[][] lines, int columns, int rows, out int total, out char[][] newgrid)
{
    total = 0;
    newgrid = new char[rows][];
    for (int j = 0; j < rows; j++) newgrid[j] = new char[columns];
    for (int i = 0; i < columns; i++)
    {
        int previous = -1;
        for (int j = 0; j < rows; j++)
        {
            if (lines[j][i] == '.')
            {
                newgrid[j][i] = lines[j][i];
            }
            if (lines[j][i] == '#')
            {
                newgrid[j][i] = lines[j][i];
                previous = j;
            }
            if (lines[j][i] == 'O')
            {
                newgrid[j][i] = '.';
                newgrid[previous + 1][i] = 'O';
                previous = previous + 1;
            }
        }
    }
}

static void SouthShuffle(char[][] lines, int columns, int rows, out int total, out char[][] newgrid)
{
    total = 0;
    newgrid = new char[rows][];
    for (int j = 0; j < rows; j++) newgrid[j] = new char[columns];
    for (int i = 0; i < columns; i++)
    {
        int previous = rows;
        for (int j = rows-1; j >= 0; j--)
        {
            if (lines[j][i] == '.')
            {
                newgrid[j][i] = lines[j][i];
            }
            if (lines[j][i] == '#')
            {
                newgrid[j][i] = lines[j][i];
                previous = j;
            }
            if (lines[j][i] == 'O')
            {
                newgrid[j][i] = '.';
                newgrid[previous - 1][i] = 'O';
                previous = previous - 1;
            }
        }
    }
}

static void EastShuffle(char[][] lines, int columns, int rows, out int total, out char[][] newgrid)
{
    total = 0;
    newgrid = new char[rows][];
    for (int j = 0; j < rows; j++) newgrid[j] = new char[columns];
    for (int j = 0; j < rows; j++)
    {
        int previous = columns;
        for (int i = columns - 1; i >= 0; i--)
        {
            if (lines[j][i] == '.')
            {
                newgrid[j][i] = lines[j][i];
            }
            if (lines[j][i] == '#')
            {
                newgrid[j][i] = lines[j][i];
                previous = i;
            }
            if (lines[j][i] == 'O')
            {
                newgrid[j][i] = '.';
                newgrid[j][previous - 1] = 'O';
                previous = previous - 1;
            }
        }
    }
}

static void WestShuffle(char[][] lines, int columns, int rows, out int total, out char[][] newgrid)
{
    total = 0;
    newgrid = new char[rows][];
    for (int j = 0; j < rows; j++) newgrid[j] = new char[columns];
    for (int j = 0; j < rows; j++)
    {
        int previous = -1;
        for (int i = 0; i < columns; i++)
        {
            if (lines[j][i] == '.')
            {
                newgrid[j][i] = lines[j][i];
            }
            if (lines[j][i] == '#')
            {
                newgrid[j][i] = lines[j][i];
                previous = i;
            }
            if (lines[j][i] == 'O')
            {
                newgrid[j][i] = '.';
                newgrid[j][previous + 1] = 'O';
                previous = previous + 1;
            }
        }
    }
}

static int Score(char[][] grid)
{
    int total = 0;
    int rows = grid.Length;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < grid[i].Length; j++)
        {
            if (grid[i][j] == 'O')
                total += rows - i;
        }
    }
    return total;
}

