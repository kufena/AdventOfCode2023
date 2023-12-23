// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);

Part1(lines);

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

    for (int i = 0; i < count; i++)
    {
        if (bricksA[i].Item1 != bricksB[i].Item1)
        { // x direction?
            Console.WriteLine($"{i}:: in x direction, {Math.Abs(bricksA[i].Item1 - bricksB[i].Item1)}");

        }
        if (bricksA[i].Item2 != bricksB[i].Item2)
        { // y direction?
            Console.WriteLine($"{i}:: in y direction, {Math.Abs(bricksA[i].Item2 - bricksB[i].Item2)}");
        }
        if (bricksA[i].Item3 != bricksB[i].Item3)
        { // z direction?
            Console.WriteLine($"{i}:: in z direction, {Math.Abs(bricksA[i].Item3 - bricksB[i].Item3)}");
            if (bricksA[i].Item3 > bricksB[i].Item3)
            {
                var sw = bricksA[i];
                bricksA[i] = bricksB[i];
                bricksB[i] = sw;
            }
        }
    }

    Console.WriteLine("Fwoop!");
}
