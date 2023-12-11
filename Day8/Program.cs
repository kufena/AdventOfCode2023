// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
//Part2(lines);
Part2b(lines);
//Part2c(lines);


static void Part1(string[] lines)
{
    var instructions = lines[0];
    Dictionary<string, (string, string)> map = new();
    for (int i = 2; i < lines.Length; i++)
    {
        var spl1 = lines[i].Split('=',StringSplitOptions.RemoveEmptyEntries);
        var spl2 = spl1[1].Split(new char[] { '(', ',', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        map.Add(spl1[0].Trim(), (spl2[0].Trim(),spl2[1].Trim()));
    }

    int l = 0;
    int steps = 0;
    string current = "AAA";
    while (true)
    {
        if (current.Equals("ZZZ"))
            break;

        (var left, var right) = map[current];
        if (instructions[l] == 'L') 
            current = left;
        else 
            current = right;
        l = (l + 1) % instructions.Length;
        steps++;
    }

    Console.WriteLine($"Took {steps} steps.");
}

// BRUTE FORCE.
// This is a good way of doing it but might take as long as the universe to complete.
static void Part2(string[] lines)
{
    var instructions = lines[0];
    Dictionary<string, (string, string)> map = new();
    List<string> startlist = new();
    for (int i = 2; i < lines.Length; i++)
    {
        var spl1 = lines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
        var spl2 = spl1[1].Split(new char[] { '(', ',', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        map.Add(spl1[0].Trim(), (spl2[0].Trim(), spl2[1].Trim()));
        if (spl1[0].Trim().EndsWith('A')) startlist.Add(spl1[0].Trim());
    }

    Console.WriteLine($"We have {startlist.Count} starts.");
    var starts = startlist.ToArray();
    int l = 0;
    long steps = 0;
    while (true)
    {
        bool done = true;
        for (int i = 0; i < starts.Length; i++)
        {
            if (!starts[i].EndsWith("Z")) done = false;
        }
        if (done) break;

        for (int i = 0; i < starts.Length; i++)
        {
            (var left, var right) = map[starts[i]];
            if (instructions[l] == 'L')
                starts[i] = left;
            else
                starts[i] = right;
        }

        l = (l + 1) % instructions.Length;
        steps++;
        if (steps % 1000000 == 0) Console.WriteLine($"{steps}");
    }

    Console.WriteLine($"Took {steps} steps.");
}

static void Part2b(string[] lines)
{
    var instructions = lines[0];
    Dictionary<string, (string, string)> map = new();
    List<string> startlist = new();
    for (int i = 2; i < lines.Length; i++)
    {
        var spl1 = lines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
        var spl2 = spl1[1].Split(new char[] { '(', ',', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        map.Add(spl1[0].Trim(), (spl2[0].Trim(), spl2[1].Trim()));
        if (spl1[0].Trim().EndsWith('A')) startlist.Add(spl1[0].Trim());
    }

    Console.WriteLine($"We have {startlist.Count} starts.");

    Dictionary<string, List<long>> startToSteps = new();
    long total = 1;

    Dictionary<string, long> finalSteps = new();
    Dictionary<string, List<long>> steppedEnds = new();
    Dictionary<string, long> cycleStarts = new();
    Dictionary<string, List<(int,string)>> actualSteps = new();

    // for each start, find its cylce point and all end points found in that cycle.
    // we'll never see anything other than those points in future.
    foreach (var start in startlist)
    {
        // look for cmd and current repetition.
        HashSet<(int, string)> cache = new();
        Dictionary<(int, string), long> cache_points = new();
        Dictionary<string, List<long>> endSteps = new();
        int l = 0;
        int steps = 0;
        string current = start;
        cache.Add((instructions.Length - 1, current));
        actualSteps.Add(start, new());
        cache_points.Add((instructions.Length - 1, current), 0);

        while (true)
        {
            (var left, var right) = map[current];
            if (instructions[l] == 'L')
                current = left;
            else
                current = right;

            if (current.EndsWith("Z"))
            { // store
                if (endSteps.ContainsKey(current)) endSteps[current].Add(steps + 1);
                else endSteps.Add(current, new List<long>() { steps + 1 });
            }

            if (cache.Contains((l, current))) // it's a repeat, we're done.
            {
                if (current.EndsWith("Z")) Console.WriteLine("cache point is also and end point.");
                long firstpoint = cache_points[(l, current)];
                finalSteps.Add(start, (steps - firstpoint));
                cycleStarts.Add(start, firstpoint);
                break;
            }

            cache.Add((l, current));
            cache_points.Add((l, current), steps);
            actualSteps[start].Add((l, current));

            l = (l + 1) % instructions.Length;
            steps++;
        }

        Console.WriteLine($"{start} took {steps} steps, cycle is {finalSteps[start]}.");
        List<long> allEnds = new();
        foreach (var pair in endSteps)
        {
            foreach (var rl in pair.Value)
            {
                if (rl >= (steps - finalSteps[start]))
                    allEnds.Add(rl);
            }
        }
        steppedEnds.Add(start, allEnds);
    }

    // Process results.
    // I think this works because the point where each start cycles happens to be a point
    // which ends with a Z, but I can't be sure of that, or whether they cycle from the start;
    // in fact I don't think they do.  Anyhoo, it's a star!

    var circuits = steppedEnds.ToArray();
    long last = circuits[0].Value.First();
    for (int i = 1; i < circuits.Length; i++)
    {
        last = LCM(last, circuits[i].Value.First());
    }
    Console.WriteLine(last);
}

static long LCM(long a, long b)
{
    long partial = Math.Abs(a * b);
    long gcd = GCD(a, b);
    return (partial / gcd);
}

/*
 * function gcd(a, b)
    while a ≠ b 
        if a > b
            a := a − b
        else
            b := b − a
    return a
*/
static long GCD(long a, long b)
{
    while (a != b)
    {
        if (a > b)
        {
            a = a - b;
        }
        else
            b = b - a;
    }
    return a;
}
