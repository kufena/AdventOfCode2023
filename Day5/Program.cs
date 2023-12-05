// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    var seedsplits = lines[0].Split(':', StringSplitOptions.RemoveEmptyEntries);
    var seeds = new List<long>();
    var seedstrs = seedsplits[1].Split(' ',StringSplitOptions.RemoveEmptyEntries);
    foreach (var seedstr in seedstrs) seeds.Add(long.Parse(seedstr));
    List<Map> maps = new();

    int ind = 2;
    while (true)
    {
        // get to/from
        Map m = new Map();
        var spl1 = lines[ind].Split(' ');
        Console.WriteLine(lines[ind]);
        Console.WriteLine(spl1[0]);
        var spl2 = spl1[0].IndexOf("-to-");
        var from = spl1[0].Substring(0, spl2);
        var to = spl1[0].Substring(spl2 + 4);
        m.from = from;
        m.to = to;

        ind++;
        while (ind < lines.Length && lines[ind].Trim() != "")
        {
            var spl3 = lines[ind].Split(' ');
            long target = long.Parse(spl3[0]);
            long source = long.Parse(spl3[1]);
            long length = long.Parse(spl3[2]);
            Range r = new Range { target = target, source = source, length = length };
            m.ranges.Add(r);
            ind++;
        }
        maps.Add(m);
        if (ind >= lines.Length) break;
        ind++;
    }

    Console.WriteLine($"{seeds.Count} seeds and {maps.Count} maps.");

    long[] vals = new long[seeds.Count];
    for (int i = 0; i < seeds.Count; i++) vals[i] = seeds[i];
    foreach (var map in maps)
    {
        for (int i = 0; i < seeds.Count; i++)
        {
            vals[i] = map.map(vals[i]);    
        }
    }
    for (int i = 0; i < seeds.Count; i++)
    {
        Console.WriteLine($"{seeds[i]} goes to {vals[i]}");
    }
    Array.Sort(vals);
    Console.WriteLine($"Lowest is {vals[0]}");

}
static void Part2(string[] lines)
{
    var seedsplits = lines[0].Split(':', StringSplitOptions.RemoveEmptyEntries);
    var seeds = new List<(long,long)>();
    var seedstrs = seedsplits[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < seedstrs.Length; i += 2)
    {
        long seedstart = long.Parse(seedstrs[i]);
        long seedrange = long.Parse(seedstrs[i + 1]);
        seeds.Add((seedstart, seedrange));
    }

    List<Map> maps = new();

    int ind = 2;
    while (true)
    {
        // get to/from
        Map m = new Map();
        var spl1 = lines[ind].Split(' ');
        Console.WriteLine(lines[ind]);
        Console.WriteLine(spl1[0]);
        var spl2 = spl1[0].IndexOf("-to-");
        var from = spl1[0].Substring(0, spl2);
        var to = spl1[0].Substring(spl2 + 4);
        m.from = from;
        m.to = to;

        ind++;
        while (ind < lines.Length && lines[ind].Trim() != "")
        {
            var spl3 = lines[ind].Split(' ');
            long target = long.Parse(spl3[0]);
            long source = long.Parse(spl3[1]);
            long length = long.Parse(spl3[2]);
            Range r = new Range { target = target, source = source, length = length };
            m.ranges.Add(r);
            ind++;
        }
        maps.Add(m);
        if (ind >= lines.Length) break;
        ind++;
    }

    Console.WriteLine($"{seeds.Count} seeds and {maps.Count} maps.");

    long lowest = long.MaxValue;
    Task<long>[] runs = new Task<long>[seeds.Count];
    int k = 0;
    foreach ((long sstart, long srange) in seeds)
    {
        runs[k] = Task<long>.Run(() =>
        {
            long lowlow = long.MaxValue;
            Console.WriteLine($"Ranging from {sstart} to {sstart + srange}");
            for (long seed = sstart; seed < sstart + srange; seed++)
            {
                long val = seed;
                foreach (var map in maps)
                {
                    val = map.map(val);
                }
                if (val < lowlow) lowlow = val;
            }
            Console.WriteLine($"{sstart} to {srange} lowest is {lowlow}");
            return lowlow;
        });
        k++;
    }
    Task.WaitAll(runs);
    foreach (var t in runs)
    {
        if (t.Result < lowest) lowest = t.Result;
    }
    Console.WriteLine($"Lowest is {lowest}");

}

public class Map
{
    public Map() { ranges = new(); }

    public string from { get; set; }
    public string to { get; set; }
    public List<Range> ranges { get; set; }

    public long map(long x) 
    {
        foreach (var range in ranges)
        {
            if (range.in_range(x)) return range.map(x);
        }
        return x; 
    }
}

public class Range {

    public Range()
    {
    }
    public long source { get; set; }
    public long target { get; set; }
    public long length { get; set; }

    public long source_end { get => source + (length - 1); }
    public long target_end { get => target + (length - 1); }

    public bool in_range(long x) { return (x >= source && x <= source_end); }
    public long map(long x) { return target + (x - source); }
}

