// See https://aka.ms/new-console-template for more information

using System.Diagnostics.CodeAnalysis;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int xext = lines[0].Length;
int yext = lines.Length;

int[][] grid = new int[yext][];
bool[][] visited = new bool[yext][];
for (int i = 0; i < yext; i++)
{
    grid[i] = new int[xext];
    visited[i] = new bool[xext];
    for (int j = 0; j < xext; j++)
    {
        grid[i][j] = lines[i][j] - '0';
        visited[i][j] = false;
    }
}

Dictionary<(int, int, int, int, int), long> cache = new();
cache.Add((yext - 1, xext - 1, 0, 1, 1), grid[yext - 1][xext - 1]);
cache.Add((yext - 1, xext - 1, 0, 1, 2), grid[yext - 1][xext - 1]);
cache.Add((yext - 1, xext - 1, 0, 1, 3), grid[yext - 1][xext - 1]);
cache.Add((yext - 1, xext - 1, -1, 0, 1), grid[yext - 1][xext - 1]);
cache.Add((yext - 1, xext - 1, -1, 0, 2), grid[yext - 1][xext - 1]);
cache.Add((yext - 1, xext - 1, -1, 0, 3), grid[yext - 1][xext - 1]);

long total = Part1(grid, 0, 0, (1,0), 0, 1);
Console.WriteLine(total);
Console.WriteLine(long.MaxValue);

long Part1(int[][] grid, int y, int x, (int,int) direction, int count, int depth)
{
    (int dy, int dx) = direction;
    if (cache.ContainsKey((y, x, dy, dx, count)))
        return cache[(y, x, dy, dx, count)];

    visited[y][x] = true; // we don't want cycles.

    if (y == yext - 1 && x == xext - 1) // are we in the bottom right yet?
        return grid[y][x];

    long val;
    HashSet<(int, int, int)> directionsToFollow = new();
    if (dy != 0)
    {
        directionsToFollow.Add((0, 1, 1));
        directionsToFollow.Add((0, -1, 1));

        if (count < 3)
            directionsToFollow.Add((dy, dx, count + 1));
        
    }
    else
    {
        directionsToFollow.Add((1, 0, 1));
        directionsToFollow.Add((-1, 0, 1));
        if (count < 3)
            directionsToFollow.Add((dy, dx, count + 1));
    }

    List<long> subs = new();
    foreach ((int dyp, int dxp, int countp) in directionsToFollow)
    {
        if (x + dxp >= 0 && x + dxp < xext && y + dyp >= 0 && y + dyp < yext) // not off the grid
        {
            if (!visited[y + dyp][x + dxp]) {
                var l = Part1(grid, y + dyp, x + dxp, (dyp, dxp), countp, depth+1);
                subs.Add(l);
            }
        }
    }
    var min = subs.Count > 0 ? subs.Min() : -1;
    val = min + grid[y][x];
    cache.Add((y, x, dy, dx, count), val);
    return val;
}