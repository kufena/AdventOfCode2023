// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    long total = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        bool inNum = false;
        long curnum = 0;
        int startind = 0;
        for (int j = 0; j < lines[i].Length; j++)
        {
            if (Char.IsDigit(lines[i][j]))
            {
                if (!inNum)
                {
                    inNum = true;
                    startind = j == 0 ? 0 : j - 1;
                }
                curnum = (curnum * 10) + (lines[i][j] - '0');
            }
            else
            {
                // it's not a digit.
                if (inNum)
                {
                    inNum = false; // we're not in one now
                    // need to test for surround by character.
                    bool charFound = false;
                    int lowi = i == 0 ? 0 : i - 1;
                    int highi = i == lines.Length - 1 ? i : i + 1;
                    for (int k = startind; k <= j; k++)
                    {
                        for (int l = lowi; l <= highi; l++)
                        {
                            if (lines[l][k] != '.' && !Char.IsDigit(lines[l][k]))
                            {
                                charFound = true;
                                break;
                            }
                        }
                    }
                    if (charFound) total += curnum;
                    curnum = 0;
                    startind = 0;
                }
            }
        }

        // need to check that we weren't in a num and it hasn't a char
        if (inNum)
        {
            inNum = false; // we're not in one now
                           // need to test for surround by character.
            bool charFound = false;
            int lowi = i == 0 ? 0 : i - 1;
            int highi = i == lines.Length - 1 ? i : i + 1;
            for (int k = startind; k < lines[i].Length; k++)
            {
                for (int l = lowi; l <= highi; l++)
                {
                    if (lines[l][k] != '.' && !Char.IsDigit(lines[l][k]))
                    {
                        charFound = true;
                        break;
                    }
                }
            }
            if (charFound) total += curnum;
            curnum = 0;
            startind = 0;
        }
        Console.WriteLine(total);
    }
}

static void Part2(string[] lines)
{
    Dictionary<(int, int), List<long>> gears = new();
    long total = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        bool inNum = false;
        long curnum = 0;
        int startind = 0;
        for (int j = 0; j < lines[i].Length; j++)
        {
            if (Char.IsDigit(lines[i][j]))
            {
                if (!inNum)
                {
                    inNum = true;
                    startind = j == 0 ? 0 : j - 1;
                }
                curnum = (curnum * 10) + (lines[i][j] - '0');
            }
            else
            {
                // it's not a digit.
                if (inNum)
                {
                    inNum = false; // we're not in one now
                    // need to test for surround by character.
                    bool charFound = false;
                    int lowi = i == 0 ? 0 : i - 1;
                    int highi = i == lines.Length - 1 ? i : i + 1;
                    for (int k = startind; k <= j; k++)
                    {
                        for (int l = lowi; l <= highi; l++)
                        {
                            if (lines[l][k] == '*')
                            {
                                var gearind = (l, k);
                                if (gears.ContainsKey(gearind))
                                    gears[gearind].Add(curnum);
                                else
                                    gears.Add(gearind, new List<long>() { curnum });
                            }
                        }
                    }
                    curnum = 0;
                    startind = 0;
                }
            }
        }

        // need to check that we weren't in a num and it hasn't a char
        if (inNum)
        {
            inNum = false; // we're not in one now
                           // need to test for surround by character.
            bool charFound = false;
            int lowi = i == 0 ? 0 : i - 1;
            int highi = i == lines.Length - 1 ? i : i + 1;
            for (int k = startind; k < lines[i].Length; k++)
            {
                for (int l = lowi; l <= highi; l++)
                {
                    if (lines[l][k] == '*')
                    {
                        var gearind = (l, k);
                        if (gears.ContainsKey(gearind))
                            gears[gearind].Add(curnum);
                        else
                            gears.Add(gearind, new List<long>() { curnum });
                    }
                }
            }
            curnum = 0;
            startind = 0;
        }
    }
    foreach (var (gearind, numsfound) in gears)
    {
        Console.WriteLine($"{gearind}: {numsfound.Count}");
        if (numsfound.Count > 2)
            Console.WriteLine("£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££");
        if (numsfound.Count == 2)
            total += (numsfound[0] * numsfound[1]);
    }
    Console.WriteLine(total);
 }
