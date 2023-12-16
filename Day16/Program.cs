// See https://aka.ms/new-console-template for more information
using System.Numerics;
using System.Text;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

Part1(lines);


static void Part1(string[] lines)
{
    int yext = lines.Length;
    int xext = lines[0].Length;

    SpaceType[][] grid = new SpaceType[yext][];
    for (int i = 0; i < lines.Length; i++)
    {
        grid[i] = new SpaceType[xext];
        for (int j = 0; j < xext; j++)
        {
            if (lines[i][j] == '.')
                grid[i][j] = SpaceType.Space;
            if (lines[i][j] == '\\')
                grid[i][j] = SpaceType.BackMirror;
            if (lines[i][j] == '/')
                grid[i][j] = SpaceType.ForwardMirror;
            if (lines[i][j] == '-')
                grid[i][j] = SpaceType.HorizontalSplitter;
            if (lines[i][j] == '|')
                grid[i][j] = SpaceType.VerticalSplitter;
        }
    }

    (int, int) rightdir = (1, 0);
    (int, int) leftdir = (-1, 0);
    (int, int) downdir = (0, 1);
    (int, int) updir = (0, -1);

    int maxTot = 0;
    int subTot = 0;
    for (int i = 0; i < xext; i++)
    {
        subTot = PerformJourney(yext, xext, grid, i, 0, downdir);
        if (subTot > maxTot) maxTot = subTot;
    }
    for (int i = 0; i < xext; i++)
    {
        subTot = PerformJourney(yext, xext, grid, i, yext-1, updir);
        if (subTot > maxTot) maxTot = subTot;
    }
    for (int i = 0; i < xext; i++)
    {
        subTot = PerformJourney(yext, xext, grid, 0, i, rightdir);
        if (subTot > maxTot) maxTot = subTot;
    }
    for (int i = 0; i < xext; i++)
    {
        subTot = PerformJourney(yext, xext, grid, xext-1, i, leftdir);
        if (subTot > maxTot) maxTot = subTot;
    }
    Console.WriteLine($"Total = {maxTot}");
}

static int PerformJourney(int yext, int xext, SpaceType[][] grid, int startx, int starty, (int, int) startdir)
{
    bool[][] energized;
    energized = new bool[yext][];
    for (int i = 0; i < yext; i++)
    {
        energized[i] = new bool[xext];
        for (int j = 0; j < xext; j++)
            energized[i][j] = false;
    }
    Stack<(int, int, (int, int))> toComplete = new();
    Dictionary<(int, int), (int, int)> gridToDirections = new();
    toComplete.Push((startx, starty, startdir));
    bool start = true;
    energized[starty][startx] = true;

    HashSet<(int, int, int, int)> cycleCheck = new();
    while (toComplete.Count > 0)
    {
        Console.WriteLine("Doing a thang.");
        (var x, var y, (int dx, int dy)) = toComplete.Pop();
        while (start || (x >= 0 && x < xext && y >= 0 && y < yext))
        {
            gridToDirections[(y, x)] = (dx, dy);
            start = false;
            energized[y][x] = true;
            //Console.WriteLine($"{y} {x} {grid[y][x]}");
            switch (grid[y][x])
            {
                case SpaceType.Space:
                    x += dx;
                    y += dy;
                    break;
                case SpaceType.VerticalSplitter:
                    if (dx != 0)
                    {
                        toComplete.Push((x, y - 1, (0, -1)));
                        dx = 0;
                        dy = 1;
                        y += 1;
                    }
                    else
                    {
                        x += dx;
                        y += dy;
                    }
                    break;
                case SpaceType.HorizontalSplitter:
                    if (dy != 0)
                    {
                        toComplete.Push((x + 1, y, (1, 0)));
                        dx = -1;
                        dy = 0;
                        x -= 1;
                    }
                    else
                    {
                        x += dx;
                        y += dy;
                    }
                    break;
                case SpaceType.ForwardMirror:
                    if (dx != 0)
                    {
                        dy = -dx;
                        dx = 0;
                    }
                    else
                    {
                        dx = -dy;
                        dy = 0;
                    }
                    x += dx;
                    y += dy;
                    break;
                case SpaceType.BackMirror:
                    int k2 = dx;
                    if (dy != 0)
                    {
                        dx = dy;
                        dy = 0;
                    }
                    else
                    {
                        dy = dx;
                        dx = 0;
                    }
                    x += dx;
                    y += dy;
                    break;
            }
            if (cycleCheck.Contains((x, y, dx, dy)))
            {
                //Console.WriteLine($"Breaking for cycle. {y} {x}");
                break;
            }
            cycleCheck.Add((x, y, dx, dy));
        }

    }

    int tot = 0;
    for (int i = 0; i < yext; i++)
    {
        //StringBuilder sb = new StringBuilder();
        for (int j = 0; j < xext; j++)
        {
            tot += energized[i][j] ? 1 : 0;
            //sb.Append(energized[i][j]?'#':'.');
            //if (gridToDirections.ContainsKey((i, j)))
            //{
            //    (int dx, int dy) = gridToDirections[(i, j)];
            //    if (dx == 1) sb.Append('>');
            //    if (dx == -1) sb.Append('<');
            //    if (dy == 1) sb.Append('v');
            //    if (dy == -1) sb.Append('^');
            //}
            //else
            //    sb.Append('.');
        }
        //Console.WriteLine(sb.ToString());
    }
    return tot;
}

enum SpaceType { Space = 1, BackMirror = 2, ForwardMirror = 3, VerticalSplitter = 4, HorizontalSplitter = 5 };

