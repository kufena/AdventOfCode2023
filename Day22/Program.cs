// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);


static void Part1(string[] lines)
{
    var bricksA = new (int, int, int)[lines.Length];
    var bricksB = new (int, int, int)[lines.Length];
    int count = lines.Length;

    for (int i = 0; i < lines.Length; i++)
    {
        var exts = lines[i].Split("~", StringSplitOptions.RemoveEmptyEntries);
        var aS = exts[0].Split(",", StringSplitOptions.RemoveEmptyEntries);
        int[] aIs = new int[aS.Length];
        for (int j = 0; j < aS.Length; j++) aIs[j] = int.Parse(aS[j]);
        var bS = exts[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
        int[] bIs = new int[bS.Length];
        for (int j = 0; j < bS.Length; j++) bIs[j] = int.Parse(bS[j]);
        bricksA[i] = (aIs[0], aIs[1], aIs[2]);
        bricksB[i] = (bIs[0], bIs[1], bIs[2]);
    }

    SortedDictionary<int, List<(int, int, int, int, int, int, int,int,int,int, int)>> yToBrick = new();
    int lowx = 0;
    int highx = 0;
    int lowy = 0;
    int highy = 0;
    int highz = 0;
    for (int i = 0; i < count; i++)
    {

//        if (bricksA[i] == bricksB[i]) // it's a single block thang.
//        {
//            Console.WriteLine($"ITS ALL THE BLOOOOOOOOOOOOOODY SAME BLOW THE DOORS OFF WHAT WHAT WHAT {bricksA[i]}");
//        }

        int dx = 0;
        int dy = 0;
        int dz = 0;
        if (bricksA[i].Item1 < lowx) lowx = bricksA[i].Item1;
        if (bricksB[i].Item1 > highx) highx = bricksB[i].Item1;

        if (bricksA[i].Item2 < lowy) lowy = bricksA[i].Item2;
        if (bricksB[i].Item2 > highy) highy = bricksB[i].Item2;

        if (bricksB[i].Item3 > highz) highz = bricksB[i].Item3;
        int size = -1;
        if (bricksA[i].Item1 != bricksB[i].Item1)
        { // x direction?
            size = Math.Abs(bricksA[i].Item1 - bricksB[i].Item1) + 1;
            //Console.WriteLine($"{i}:: in x direction, {Math.Abs(bricksA[i].Item1 - bricksB[i].Item1)}");
            dx = 1;

        }
        else if (bricksA[i].Item2 != bricksB[i].Item2)
        { // y direction?
            size = Math.Abs(bricksA[i].Item2 - bricksB[i].Item2) + 1;
            //Console.WriteLine($"{i}:: in y direction, {Math.Abs(bricksA[i].Item2 - bricksB[i].Item2)}");
            dy = 1;
        }
        else if (bricksA[i].Item3 != bricksB[i].Item3)
        { // z direction?
            size = Math.Abs(bricksA[i].Item3 - bricksB[i].Item3) + 1;
            //Console.WriteLine($"{i}:: in z direction, {Math.Abs(bricksA[i].Item3 - bricksB[i].Item3)}");
            if (bricksA[i].Item3 > bricksB[i].Item3)
            {
                var sw = bricksA[i];
                bricksA[i] = bricksB[i];
                bricksB[i] = sw;
            }
            dz = 1;
        }
        else // it's a one block job.
        {
            size = 1;
        }

        if (!yToBrick.ContainsKey(bricksA[i].Item3))
        {
            yToBrick.Add(bricksA[i].Item3, new());
        }
        yToBrick[bricksA[i].Item3].Add((bricksA[i].Item1, bricksA[i].Item2, bricksA[i].Item3,
                                        bricksB[i].Item1, bricksB[i].Item2, bricksB[i].Item3,
                                        size, dx, dy, dz, i));
    }

    int[][] grid = new int[highy+1][];
    int[][][] actual = new int[highz + 1][][];
    for (int k = 0; k < highz + 1; k++)
    {
        actual[k] = new int[highy + 1][];
        for (int i = 0; i < highy + 1; i++)
        {
            actual[k][i] = new int[highx + 1];
            for (int j = 0; j < highx + 1; j++)
            {
                actual[k][i][j] = -1;
            }
        }
    }

    List<int>[] supporting = new List<int>[count];
    int[] supportedby = new int[count];
    for (int c = 0; c < count; c++)
    {
        supporting[c] = new();
        supportedby[c] = 0;
    }

    for (int i = 0; i < highy+1; i++)
    {
        grid[i] = new int[highx+1];
        for (int j = 0; j < highx + 1; j++)
        {
            grid[i][j] = 0;
        }
    }

    int disintegratable = 0;

    foreach (var ok in yToBrick)
    {
        foreach (var k in ok.Value) {
            (int ax, int ay, int az, int bx, int by, int bz, int sz, int dx, int dy, int dz, int index) = k;
            HashSet<int> undies = new();
            if (dz > 0)
            {
                int currh = grid[ay][ax] + 1;
                grid[ay][ax] += sz; // just increase the height at that spot by size.
                for (int q = 0; q < sz; q++)
                {
                    actual[currh + q][ay][ax] = index;
                }
                if (actual[currh - 1][ay][ax] != -1) undies.Add(actual[currh - 1][ay][ax]);
            }
            else
            {
                int max = 0;
                for (int q = 0; q < sz; q++)
                {
                    if (grid[ay + (dy*q)][ax + (dx*q)] > max)
                        max = grid[ay + (dy*q)][ax + (dx*q)];
                }
                int dccount = 0;
                for (int q = 0; q < sz; q++)
                {
                    int ddy = q * dy;
                    int ddx = q * dx;
                    if (grid[ay + ddy][ax + ddx] == max)
                        dccount += 1;
                    grid[ay + ddy][ax + ddx] = max + 1;
                    actual[max + 1][ay + ddy][ax + ddx] = index;

                    if (actual[max][ay + ddy][ax + ddx] != -1) undies.Add(actual[max][ay + ddy][ax + ddx]);
                }
            }
            foreach (var x in undies) supporting[x].Add(index);
            supportedby[index] = undies.Count;
        }
    }

    for (int c = 0; c < count; c++)
    {
        //Console.WriteLine($"{c} is supporting {supporting[c].Count} but supported by {supportedby[c]}");
        if (supporting[c].Count == 0)
        {
            disintegratable += 1;
            //Console.WriteLine("Ok to disintegrate as supports nothing.");
        }
        else
        {
            bool okToDis = true;
            foreach (var x in supporting[c])
            {
                if (supportedby[x] == 1)
                    okToDis = false;
            }
            if (okToDis)
            {
                disintegratable += 1;
                //Console.WriteLine("Ok to disintegrate as supports have >1 support each");
            }
        }
    }
    Console.WriteLine($"Fwoop! {disintegratable}");
}

//
// There's a lot of overlap that could be shared but I can't be asked to  refactor it just
// yet so here we go - count the falling bricks, rather than disintegratable bricks.
static void Part2(string[] lines)
{
    var bricksA = new (int, int, int)[lines.Length];
    var bricksB = new (int, int, int)[lines.Length];
    int count = lines.Length;

    for (int i = 0; i < lines.Length; i++)
    {
        var exts = lines[i].Split("~", StringSplitOptions.RemoveEmptyEntries);
        var aS = exts[0].Split(",", StringSplitOptions.RemoveEmptyEntries);
        int[] aIs = new int[aS.Length];
        for (int j = 0; j < aS.Length; j++) aIs[j] = int.Parse(aS[j]);
        var bS = exts[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
        int[] bIs = new int[bS.Length];
        for (int j = 0; j < bS.Length; j++) bIs[j] = int.Parse(bS[j]);
        bricksA[i] = (aIs[0], aIs[1], aIs[2]);
        bricksB[i] = (bIs[0], bIs[1], bIs[2]);
    }

    SortedDictionary<int, List<(int, int, int, int, int, int, int, int, int, int, int)>> yToBrick = new();
    int lowx = 0;
    int highx = 0;
    int lowy = 0;
    int highy = 0;
    int highz = 0;
    for (int i = 0; i < count; i++)
    {

        //        if (bricksA[i] == bricksB[i]) // it's a single block thang.
        //        {
        //            Console.WriteLine($"ITS ALL THE BLOOOOOOOOOOOOOODY SAME BLOW THE DOORS OFF WHAT WHAT WHAT {bricksA[i]}");
        //        }

        int dx = 0;
        int dy = 0;
        int dz = 0;
        if (bricksA[i].Item1 < lowx) lowx = bricksA[i].Item1;
        if (bricksB[i].Item1 > highx) highx = bricksB[i].Item1;

        if (bricksA[i].Item2 < lowy) lowy = bricksA[i].Item2;
        if (bricksB[i].Item2 > highy) highy = bricksB[i].Item2;

        if (bricksB[i].Item3 > highz) highz = bricksB[i].Item3;
        int size = -1;
        if (bricksA[i].Item1 != bricksB[i].Item1)
        { // x direction?
            size = Math.Abs(bricksA[i].Item1 - bricksB[i].Item1) + 1;
            //Console.WriteLine($"{i}:: in x direction, {Math.Abs(bricksA[i].Item1 - bricksB[i].Item1)}");
            dx = 1;

        }
        else if (bricksA[i].Item2 != bricksB[i].Item2)
        { // y direction?
            size = Math.Abs(bricksA[i].Item2 - bricksB[i].Item2) + 1;
            //Console.WriteLine($"{i}:: in y direction, {Math.Abs(bricksA[i].Item2 - bricksB[i].Item2)}");
            dy = 1;
        }
        else if (bricksA[i].Item3 != bricksB[i].Item3)
        { // z direction?
            size = Math.Abs(bricksA[i].Item3 - bricksB[i].Item3) + 1;
            //Console.WriteLine($"{i}:: in z direction, {Math.Abs(bricksA[i].Item3 - bricksB[i].Item3)}");
            if (bricksA[i].Item3 > bricksB[i].Item3)
            {
                var sw = bricksA[i];
                bricksA[i] = bricksB[i];
                bricksB[i] = sw;
            }
            dz = 1;
        }
        else // it's a one block job.
        {
            size = 1;
        }

        if (!yToBrick.ContainsKey(bricksA[i].Item3))
        {
            yToBrick.Add(bricksA[i].Item3, new());
        }
        yToBrick[bricksA[i].Item3].Add((bricksA[i].Item1, bricksA[i].Item2, bricksA[i].Item3,
                                        bricksB[i].Item1, bricksB[i].Item2, bricksB[i].Item3,
                                        size, dx, dy, dz, i));
    }

    int[][] grid = new int[highy + 1][];
    int[][][] actual = new int[highz + 1][][];
    for (int k = 0; k < highz + 1; k++)
    {
        actual[k] = new int[highy + 1][];
        for (int i = 0; i < highy + 1; i++)
        {
            actual[k][i] = new int[highx + 1];
            for (int j = 0; j < highx + 1; j++)
            {
                actual[k][i][j] = -1;
            }
        }
    }

    List<int>[] supporting = new List<int>[count];
    HashSet<int>[] supportedby = new HashSet<int>[count];
    for (int c = 0; c < count; c++)
    {
        supporting[c] = new();
        //supportedby[c] = new();
    }

    for (int i = 0; i < highy + 1; i++)
    {
        grid[i] = new int[highx + 1];
        for (int j = 0; j < highx + 1; j++)
        {
            grid[i][j] = 0;
        }
    }

    int disintegratable = 0;

    foreach (var ok in yToBrick)
    {
        foreach (var k in ok.Value)
        {
            (int ax, int ay, int az, int bx, int by, int bz, int sz, int dx, int dy, int dz, int index) = k;
            HashSet<int> undies = new();
            if (dz > 0)
            {
                int currh = grid[ay][ax] + 1;
                grid[ay][ax] += sz; // just increase the height at that spot by size.
                for (int q = 0; q < sz; q++)
                {
                    actual[currh + q][ay][ax] = index;
                }
                if (actual[currh - 1][ay][ax] != -1) undies.Add(actual[currh - 1][ay][ax]);
            }
            else
            {
                int max = 0;
                for (int q = 0; q < sz; q++)
                {
                    if (grid[ay + (dy * q)][ax + (dx * q)] > max)
                        max = grid[ay + (dy * q)][ax + (dx * q)];
                }
                int dccount = 0;
                for (int q = 0; q < sz; q++)
                {
                    int ddy = q * dy;
                    int ddx = q * dx;
                    if (grid[ay + ddy][ax + ddx] == max)
                        dccount += 1;
                    grid[ay + ddy][ax + ddx] = max + 1;
                    actual[max + 1][ay + ddy][ax + ddx] = index;

                    if (actual[max][ay + ddy][ax + ddx] != -1) undies.Add(actual[max][ay + ddy][ax + ddx]);
                }
            }
            foreach (var x in undies) supporting[x].Add(index);
            supportedby[index] = undies;
        }
    }

    int[] fallingbricks = new int[count];
    for (int t = 0; t < count; t++) fallingbricks[t] = -1;

    //    for (int c = count - 1; c >= 0; c--)
    int tot = 0;
    for (int c = 0; c < count; c++)
    {
            //Console.WriteLine($"{c} is supporting {supporting[c].Count} but supported by {supportedby[c]}");
        if (supporting[c].Count == 0)
        {
            tot += 0;
            Console.WriteLine($"{c} removal causes no bricks to fall - supports none.");
        }
        else
        {
            HashSet<int> brickfalls = new();
            int fallers = CalculateFallingBricks(supporting, supportedby, new HashSet<int>(), fallingbricks, c,brickfalls);
            tot += brickfalls.Count; // fallers > 0 ? fallers - 1 : 0;
            Console.WriteLine($"{c} removal causes {brickfalls.Count} bricks to fall.");
        }
    }
    Console.WriteLine($"Fwoop! {disintegratable}");
    Console.WriteLine(tot);
}

static int CalculateFallingBricks(List<int>[] supporting, HashSet<int>[] supportedby, HashSet<int> seen, int[] fallingbricks, int c, HashSet<int> fallen)
{
    int fallc = 0;
    int localfall = 0;

    if (seen.Contains(c)) return 0;
    seen.Add(c);

    if (supporting[c].Count == 0)
        return 1;

    foreach (var x in supporting[c])
    {
        if (supportedby[x].Count == 1 || fallen.IsSupersetOf(supportedby[x])) 
        {
            fallen.Add(x);
            if (!seen.Contains(x))
                localfall++;
        }
    }

    foreach (var x in supporting[c])
    {
        if (fallen.Contains(x) && !seen.Contains(x))
        {
            fallc += CalculateFallingBricks(supporting, supportedby, seen, fallingbricks, x, fallen);
        }
    }

    fallingbricks[c] = fallc + localfall;
    return fallc + localfall;
}