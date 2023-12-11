// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations;
using System.Linq;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
Part2(lines);

static void Part1(string[] lines)
{
    Display(lines);
    Console.WriteLine("============================================================");

    // assume a grid.
    var xlen = lines[0].Length;
    var ylen = lines.Length;

    // Extend for rows without galaxies.
    string alldots = "";
    for (int i = 0; i < xlen; i++) alldots += ".";
    List<string> newlines = new();
    for (int j = 0; j < ylen; j++)
    {
        if (String.Equals(alldots, lines[j]))
        {
            newlines.Add(lines[j]);
            newlines.Add(lines[j]);
        }
        else
        {
            newlines.Add(lines[j]);
        }
    }
    lines = newlines.ToArray();
    ylen = lines.Length;

    // Extend for columns without galaxies.
    List<int> cols = new();
    for (int i = 0; i < xlen; i++)
    {
        bool empty = true;
        for (int j = 0; j < ylen; j++)
        {
            if (lines[j][i] != '.')
            {
                empty = false;
                break;
            }
        }
        if (empty) cols.Add(i);
    }
    if (cols.Count > 0)
    {
        int newxlen = xlen + cols.Count;
        string[] l = new string[ylen];
        for (int i = 0; i < ylen; i++)
        {
            l[i] = "";
            int prev = 0;
            foreach (var c in cols)
            {
                l[i] = l[i] + lines[i].Substring(prev, c - prev) + ".";
                prev = c;
            }
            if (prev < xlen)
                l[i] = l[i] + lines[i].Substring(prev, xlen - prev);
        }
        lines = l;
        xlen = xlen + cols.Count;
    }

    Display(lines);

    // Work.
    int distanceSum = 0;
    var coords = GalaxieCoordinates(lines, xlen, ylen);
    foreach ((int g1y, int g1x) in coords)
    {
        foreach ((int g2y, int g2x) in coords)
        {
            distanceSum += (Math.Abs(g1y - g2y) + Math.Abs(g1x - g2x));
        }
    }
    Console.WriteLine($"Distance sum is {distanceSum / 2}");
}

static void Part2(string[] lines)
{
    Display(lines);
    Console.WriteLine("============================================================");

    // assume a grid.
    var xlen = lines[0].Length;
    var ylen = lines.Length;

    // Extend for rows without galaxies.
    string alldots = "";
    for (int i = 0; i < xlen; i++) alldots += ".";
    List<int> blankRows = new List<int>();
    for (int j = 0; j < ylen; j++)
    {
        if (String.Equals(alldots, lines[j]))
        {
            blankRows.Add(j);
        }
    }
    // Extend for columns without galaxies.
    List<int> cols = new();
    for (int i = 0; i < xlen; i++)
    {
        bool empty = true;
        for (int j = 0; j < ylen; j++)
        {
            if (lines[j][i] != '.')
            {
                empty = false;
                break;
            }
        }
        if (empty) cols.Add(i);
    }

    // Work.
    const int factor = 999999; // 1000000;
    long distanceSum = 0;
    var coords = GalaxieCoordinates(lines, xlen, ylen);
    HashSet<(int, int)> repeats = new();
    int pairs = 1;
    foreach ((int g1y, int g1x) in coords)
    {
        repeats.Add((g1y, g1x));
        foreach ((int g2y, int g2x) in coords)
        {
            if (repeats.Contains((g2y,g2x))) continue;
            int rowadds = 0;
            int coladds = 0;
            if (g1x != g2x)
            {
                if (g1x < g2x)
                    rowadds = cols.Where(x => x > g1x && x <= g2x).Count();
                else
                    rowadds = cols.Where((x) => x >= g2x && x < g1x).Count();
            }
            if (g1y != g2y)
            {
                if (g1y < g2y)
                    coladds = blankRows.Where(x => x > g1y && x <= g2y).Count();
                else 
                    coladds = blankRows.Where(x => x >= g2y && x < g1y).Count();
            }
            var distance = (long) ((Math.Abs(g1y - g2y) + Math.Abs(g1x - g2x) + (factor * coladds) + (factor * rowadds)));
            Console.WriteLine($"{pairs}::{(g1y, g1x)} to {(g2y, g2x)} with {rowadds} row matches and {coladds} col mathces: {distance}");
            distanceSum += distance;
            pairs++;
        }
    }
    Console.WriteLine($"Distance sum is {distanceSum}");
}

static void Display(string[] lines)
{
    for (int i = 0; i < lines.Length; i++)
        Console.WriteLine(lines[i]);
}

static List<(int,int)> GalaxieCoordinates(string[] lines, int xlen, int ylen)
{
    var result = new List<(int,int)>();
    for (int i = 0; i < ylen; i++)
    {
        for (int j = 0; j < xlen; j++)
        {
            if (lines[i][j] == '#')
                result.Add((i, j));
        }
    }
    return result;
}
