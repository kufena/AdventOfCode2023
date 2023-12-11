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

    // Let's just look for some common ground shall we?
    var allofthem = finalSteps.Values.ToArray();
    if (allofthem == null) throw new Exception("nothing found!!!");
    long max = allofthem.Max();
    long maxfactor = 0;
    for (int j = 1; j < (max / 2); j++)
    {
        bool test = true;
        for (int i = 0; i < allofthem.Length; i++)
        {
            test = test && (allofthem[i] % j == 0);
        }
        if (test)
        {
            maxfactor = j;
            Console.WriteLine($"Factor of {j}");
        }
    }

    long greatest = 0;
    string greatestp = "";
    foreach (var pair in finalSteps)
    {
        if (pair.Value > greatest)
        {
            greatest = pair.Value;
            greatestp = pair.Key;
        }
    }

    foreach (var l in steppedEnds[greatestp])
    {
        Console.WriteLine($"{greatest} {greatestp} {l} => {l + (greatest * maxfactor)}");
    }

    // Process results.
    
    bool first = true;
    HashSet<long> myfinal = new HashSet<long>();
    foreach (var start in startlist)
    {
        Console.WriteLine("working on " + start);
        HashSet<long> hl = new();
        foreach (var l in steppedEnds[start])
        {
            if (first)
                hl.Add(l);
            else
            {
                if (myfinal.Contains(l)) hl.Add(l);
            }
        }

        long minival = 1000000000000000;
        long maxival = 100000000000000000;
        long step = finalSteps[start];
        long init = (long) Math.Floor((double)(minival / step));
        List<long> ps = new();
        foreach (var qwe in steppedEnds[start])
        {
            ps.Add(qwe + (init * step));
        }
        Console.WriteLine(init);
        long maxcompute = minival;
        while(maxcompute < maxival) 
        {
            List<long> newps = new();
            foreach (var l in ps)
            {
                long q = l + finalSteps[start];

                if (q > minival)
                {
                    if (first)
                        hl.Add(q);
                    else
                    {
                        if (myfinal.Contains(q)) hl.Add(q);
                    }
                }
                newps.Add(q);
                maxcompute = q;
            }
            ps = newps;
        }

        if (first)
        {
            first = false;
        }
        myfinal = hl;
        if (myfinal.Count == 0)
        {
            Console.WriteLine("No point as hashset is already empty.");
            break;
        }
        else
        {
            Console.WriteLine($"{myfinal.Count} ---- {myfinal.Max()}");
        }
    }

    foreach (var l in myfinal) Console.WriteLine($"Possible = {l}");
    
}