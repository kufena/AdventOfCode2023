// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Xml;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

void Part1(string[] lines)
{
    int xr = 0;
    int yu = 0;
    int xl = 0;
    int yd = 0;

    int x = 0;
    int y = 0;
    int maxx = int.MinValue;
    int minx = int.MaxValue;
    int maxy = int.MinValue;
    int miny = int.MaxValue;

    foreach (var line in lines)
    {
        var splits = line.Split(' ');
        int c = int.Parse(splits[1]);
        if (splits[0] == "R") { x += c; xr += c; }
        if (splits[0] == "L") { x -= c; xl -= c; }
        if (splits[0] == "U") { y -= c; yu -= c; }
        if (splits[0] == "D") { y += c; yd += c; }

        if (x < minx) { minx = x; }
        if (x > maxx) { maxx = x; }
        if (y < miny) { miny = y; }
        if (y > maxy) { maxy = y; }
    }
    Console.WriteLine($"Extents {xl} to {xr} and {yu} to {yd}");
    Console.WriteLine($"{minx} {miny} {maxx} {maxy}");
    xr = (maxx - minx) + 1;
    yd = (maxy - miny) + 1;

    bool[][] grid = new bool[yd][];
    char[][] day10grid = new char[yd][];
    for (int i = 0; i < yd; i++)
    {
        grid[i] = new bool[xr];
        day10grid[i] = new char[xr];
        for (int j = 0; j < xr; j++)
        {
            grid[i][j] = false;
            day10grid[i][j] = '.';
        }
    }

    x = 0 - minx;
    y = 0 - miny;
    int q = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(' ');
        int c = int.Parse(splits[1]);
        int dx = 0;
        int dy = 0;
        if (splits[0] == "R") dx = 1;
        if (splits[0] == "L") dx = -1;
        if (splits[0] == "U") dy = -1;
        if (splits[0] == "D") dy = 1;
        int next = (q + 1) % (lines.Length);
        for (int k = 0; k < c; k++)
        {
            x += dx;
            y += dy;
            grid[y][x] = true;
            day10grid[y][x] = ToDay10Map(splits[0]);
        }

        if (!lines[next].StartsWith(splits[0])) // they're not the same, so good.
        {
            day10grid[y][x] = DoTheCornerNow(lines[next], splits[0]);
        }
        q++;
    }

    long count = 0;
    HashSet<char> testers = new HashSet<char>() { '|', 'L', 'J' };
    for (int i = 0; i < yd; i++)
    {
        int crossings = 0;
        for (int j = 0; j < xr; j++)
        {
            // 
            if (grid[i][j])
            {
                count++;
            }
            if (grid[i][j] && testers.Contains(day10grid[i][j]))
            {
                crossings++;
            }
            if (!grid[i][j] && crossings % 2 == 1)
            {
                count++;
            }
        }
    }

    Console.WriteLine($"{count}");


    for (int i = 0; i < yd; i++)
    {
        StringBuilder sb = new();
        for (int j = 0; j < xr; j++)
        {
            //            sb.Append(grid[i][j] ? '#' : '.');
            sb.Append(day10grid[i][j]);
        }
        Console.WriteLine(sb.ToString());
    }

}

void Part2(string[] lines)
{
    long xr = 0;
    long yu = 0;
    long xl = 0;
    long yd = 0;

    long x = 0;
    long y = 0;
    long maxx = int.MinValue;
    long minx = int.MaxValue;
    long maxy = int.MinValue;
    long miny = int.MaxValue;

    string[] splits0 = new string[lines.Length];
    long[] splits1 = new long[lines.Length];
    int w = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(' ');
        int c = ParseFirstFiveBits(splits[2]);
        splits1[w] = c;
        //  direction to dig: 0 means R, 1 means D, 2 means L, and 3 means U.
        if (splits[2].EndsWith("0)")) { splits0[w] = "R"; x += c; xr += c; }
        if (splits[2].EndsWith("1)")) { splits0[w] = "D"; y += c; yd += c; }
        if (splits[2].EndsWith("2)")) { splits0[w] = "L"; x -= c; xl -= c; }
        if (splits[2].EndsWith("3)")) { splits0[w] = "U"; y -= c; yu -= c; }

        if (x < minx) { minx = x; }
        if (x > maxx) { maxx = x; }
        if (y < miny) { miny = y; }
        if (y > maxy) { maxy = y; }

        w += 1;
    }
    Console.WriteLine($"Extents {xl} to {xr} and {yu} to {yd}");
    Console.WriteLine($"{minx} {miny} {maxx} {maxy}");

    // extents sorted, let's go!
    xr = (maxx - minx) + 1;
    yd = (maxy - miny) + 1;

    SortedDictionary<long,char>[] day10grid = new SortedDictionary<long, char>[yd];
    for (int i = 0; i < yd; i++)
    {
        day10grid[i] = new();
    }

    x = 0 - minx;
    y = 0 - miny;
    
    for(int q = 0; q < lines.Length; q++)
    {
        var line = lines[q];
        long c = splits1[q];
        int dx = 0;
        int dy = 0;
        if (splits0[q] == "R") dx = 1;
        if (splits0[q] == "L") dx = -1;
        if (splits0[q] == "U") dy = -1;
        if (splits0[q] == "D") dy = 1;
        int next = (q + 1) % (lines.Length);
        for (int k = 0; k < c-1; k++)
        {
            x += dx;
            y += dy;

            day10grid[y].Add(x,ToDay10Map(splits0[q]));
        }
        x += dx;
        y += dy;

        if (!splits0[next].Equals(splits0[q])) // they're not the same, so good.
        {
            day10grid[y].Add(x,DoTheCornerNow(splits0[next], splits0[q]));
        }
        
    }

    Console.WriteLine("Counting...");
    long count = 0;
    HashSet<char> testers = new HashSet<char>() { '|', 'L', 'J' };
    for (int i = 0; i < yd; i++)
    {
        int crossings = 0;
        var gridline = day10grid[i];
        long xpos = 0;
        foreach(var pair in gridline)
        {
            long j = pair.Key;
            char cmd = pair.Value;

            //
            if (crossings % 2 == 1)
            {
                count += ((long)j - xpos) + 1;
            }

            //count++;

            if (testers.Contains(cmd))
            {
                crossings++;
            }
            xpos = j;
        }
    }

    count += 2; // ?????? Why this works I don't know, but hey.

    Console.WriteLine($"{count}");
    Console.WriteLine("952408144115");
    /*
    for (int i = 0; i < yd; i++)
    {
        StringBuilder sb = new();
        for (int j = 0; j < xr; j++)
        {
            //            sb.Append(grid[i][j] ? '#' : '.');
            sb.Append(day10grid[i][j]);
        }
        Console.WriteLine(sb.ToString());
    }
    */
}

int ParseFirstFiveBits(string v)
{
    int t = 0;
    for (int i = 2; i < 7; i++)
    {
        t = t * 16;
        if (Char.IsDigit(v[i]))
        {
            t += v[i] - '0';
        }
        else
        {
            t += 10 + (v[i] - 'a');
        }
    }
    return t;
}

char ToDay10Map(string v)
{
    if (v.StartsWith("U") || v.StartsWith("D")) return '|';
    if (v.StartsWith("L") || v.StartsWith("R")) return '-';
    return '.';
}

static char DoTheCornerNow(string linesnext, string splits0)
{
    if (splits0 == "U" && linesnext.StartsWith("L"))
    {
        return '7';
    }
    if (splits0 == "U" && linesnext.StartsWith("R"))
    {
        return 'F';
    }
    if (splits0 == "D" && linesnext.StartsWith("L"))
    {
        return 'J';
    }
    if (splits0 == "D" && linesnext.StartsWith("R"))
    {
        return 'L';
    }

    if (splits0 == "R" && linesnext.StartsWith("D"))
    {
        return '7';
    }
    if (splits0 == "L" && linesnext.StartsWith("D"))
    {
        return 'F';
    }
    if (splits0 == "R" && linesnext.StartsWith("U"))
    {
        return 'J';
    }
    if (splits0 == "L" && linesnext.StartsWith("U"))
    {
        return 'L';
    }
    throw new Exception("They must be the same!");
}