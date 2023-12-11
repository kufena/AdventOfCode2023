// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    bool[][] visited = new bool[lines.Length][];
    (int,int)[][] changes = new (int,int)[lines.Length][];
    int sx = 0;
    int sy = 0;
    int ylen = lines.Length;
    int xlen = lines[0].Length;
    for(int i = 0; i < ylen; i++)
    {
        visited[i] = new bool[lines[i].Length];
        changes[i] = new (int,int)[lines[i].Length];
        if (lines[i].Contains("S"))
        {
            sy = i;
            sx = lines[i].IndexOf("S");
        }
    }
    Console.WriteLine($"sx = {sx} sy = {sy}");

    List<(int, int)> directions = new List<(int, int)>() { (1, 0), (0, 1), (0, -1), (-1, 0) };
    foreach (var startdir in directions)
    {
        for (int i = 0; i < xlen; i++)
        {
            for (int j = 0; j < ylen; j++)
            {
                visited[j][i] = false;
                changes[j][i] = (0, 0);
            }
        }
        (int dx, int dy) = startdir;
        var val = CircuitFinderGeneral(sx, sy, xlen, ylen, visited, changes, lines, dx, dy);
        if (val >= 0)
        {
            for (int i = 0; i < ylen; i++)
            {
                for (int j = 0; j < xlen; j++)
                {
                    if (visited[i][j]) Console.Write(toString(changes[i][j])); // lines[i][j]);
                    else Console.Write("       "); // '.');
                }
                Console.WriteLine("");
            }
            Console.WriteLine($"{val} { val / 2}");
            break;
        }
    }
}

static void Part2(string[] lines)
{
    bool[][] visited = new bool[lines.Length][];
    (int, int)[][] changes = new (int, int)[lines.Length][];
    int sx = 0;
    int sy = 0;
    int ylen = lines.Length;
    int xlen = lines[0].Length;
    for (int i = 0; i < ylen; i++)
    {
        visited[i] = new bool[lines[i].Length];
        changes[i] = new (int, int)[lines[i].Length];
        if (lines[i].Contains("S"))
        {
            sy = i;
            sx = lines[i].IndexOf("S");
        }
    }
    Console.WriteLine($"sx = {sx} sy = {sy}");

    List<(int, int)> directions = new List<(int, int)>() { (1, 0), (0, 1), (0, -1), (-1, 0) };
    foreach (var startdir in directions)
    {
        for (int i = 0; i < xlen; i++)
        {
            for (int j = 0; j < ylen; j++)
            {
                visited[j][i] = false;
                changes[j][i] = (0, 0);
            }
        }
        (int dx, int dy) = startdir;
        HashSet<char> testers = new HashSet<char>() { '|', 'L', 'J' };
        var val = CircuitFinderGeneral(sx, sy, xlen, ylen, visited, changes, lines, dx, dy);
        if (val >= 0)
        {
            /*
             * Help from this YouTube video: https://www.youtube.com/watch?v=edVSG8Y_qf8
             * 
             * The idea is that to know if a point is inside a polygon, you draw a line from a point
             * you know IS NOT inside the shape, and draw a line to the point. If the line crosses the
             * boundary of the shape an odd number of times, then the point is inside, but if it
             * crosses an even number of times, then it is outside.
             * 
             * The trouble is that our grid may through up some spoilers which are lines that are simply
             * touches rather than entering the polygon.  For example F---7 touches the polygon but does
             * not enter it.  So, you make a decision and go either across each row or down each column,
             * looking for crossings.  If you go row by row, then a cross is when you meet | L or J, and
             * if you go down each column a cross is when you meet - F or 7.  I have the visited grid
             * which indicates points which are pipes - only test for points which aren't pipes testing
             * the number of crosses so far to determine if a point is inside or not.
             * 
             * This solution handily prints the I in the grid as it goes.
             */
            int count = 0;
            for (int i = 0; i < ylen; i++)
            {
                int crossings = 0;
                for (int j = 0; j < xlen; j++)
                {
                    //                    if (visited[i][j]) Console.Write(toString(changes[i][j])); // lines[i][j]);
                    //                    else Console.Write("       "); // '.');
                    bool inorout = false;
                    if (visited[i][j] && testers.Contains(lines[i][j])) {
                        crossings++;
                    }
                    else {
                        if (!visited[i][j] && crossings % 2 == 1)
                        {
                            count++;
                            inorout = true;
                        }
                    }
                    if (visited[i][j]) Console.Write(lines[i][j]);
                    else Console.Write(inorout ? 'I' : '.');
                }
                Console.WriteLine("");
            }
            Console.WriteLine($"{val} {val / 2}");
            Console.WriteLine(count);
            break;
        }
    }
}

static int CircuitFinderGeneral(int sx, int sy, int xlen, int ylen, bool[][] visited, (int,int)[][] changes, string[] lines, int dx, int dy) 
{
    int x = sx + dx;
    int y = sy + dy;
    int count = 1;
    changes[sy][sx] = (dx, dy);
    while (true)
    {
        if (x < 0 || x >= xlen || y < 0 || y >= ylen)
            return -1;

        if (x == sx && y == sy)
        {
            visited[sy][sx] = true;
            return count;
        }

        visited[y][x] = true;

        char c = lines[y][x];
        if (DirectionOfTravelOk(dx, dy, c))
        {
            (dx, dy) = NewDirections(dx, dy, c);
            changes[y][x] = (dx, dy);
            x += dx;
            y += dy;
            if (x < 0 || x >= xlen || y < 0 || y >= ylen)
                return -1;
            count += 1;
            if (visited[y][x]) return -1;
        }
        else
        {
            return -1;
        }
        
    }
}

static bool DirectionOfTravelOk(int dx, int dy, char c)
{
    // dx of -1 is east to west, dx of 1 is west to east
    // dy of -1 is south to north, dy of 1 is north to south
    switch (c)
    {
        case 'J': // north to west
            return (dx == 1 ) || (dy == 1);
        case 'F': // south to east
            return (dx == -1) || (dy == -1);
        case '-': // east to west
            return (dx == 1) || (dx == -1);
        case '|': // north to south
            return (dy == 1) || (dy == -1);
        case '.': // no travel
            return false;
        case 'L': // north to east
            return (dx == -1) || (dy == 1);
        case '7': // south to west
            return (dx == 1) || (dy == -1);
        default: // nope.
            return false;
    }
}

static (int,int) NewDirections(int dx, int dy, char c)
{
    switch (c)
    {
        case 'J': // north to west
            if (dx == 1) 
                return (0, -1);
            else //|| (dy == -1);
                return (-1, 0);
        case 'F': // south to east
            if (dx == -1)
                return (0, 1);
            else //|| (dy == 1);
                return (1, 0);
        case '-': // east to west
            return (dx, dy); // no change.
        case '|': // north to south
            return (dx, dy); // no change
        case '.': // no travel
            throw new Exception ("Unexpected . ");
        case 'L': // north to east
            if (dx == -1)
                return (0, -1);
            else // || (dy == -1);
                return (1, 0);
        case '7': // south to west
            if (dx == 1)
                return (0, 1);
            else // || (dy == 1);
                return (-1, 0);
        default: // nope.
            throw new Exception ($"Unexpected {c}");
    }
}

static string toString((int,int) pair) 
{
    (int x, int y) = pair;
    string res = "[";
    if (x < 0) res += $"{x},"; else res += $" {x},";
    if (y < 0) res += $"{y}]"; else res += $" {y}]";
    return res;
}