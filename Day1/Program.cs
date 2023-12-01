// See https://aka.ms/new-console-template for more information
using System.ComponentModel.Design;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

// The key to Part2 is remembering that twone is 21 not 22, because the one
// counts still.  That's it.
static void Part2(string[] lines)
{
    long total = 0;
    foreach (var ll in lines)
    {
        var line = ll.ToLower();
        List<int> vals = new();
        for (int pos = 0; pos < line.Length; pos++)
        {
            char c = line[pos];
            if (c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
            {
                vals.Add((int)(c - '0'));
                continue;
            }
            if (line.Substring(pos).StartsWith("one"))
            {
                vals.Add(1);
                //pos += 2;
                continue;
            }
            if (line.Substring(pos).StartsWith("two"))
            {
                vals.Add(2);
                //pos += 2;
                continue;
            }
            if (line.Substring(pos).StartsWith("three"))
            {
                vals.Add(3);
                //pos += 4;
                continue;
            }
            if (line.Substring(pos).StartsWith("four"))
            {
                vals.Add(4);
                //pos += 3;
                continue;
            }
            if (line.Substring(pos).StartsWith("five"))
            {
                vals.Add(5);
                //pos += 3;
                continue;
            }
            if (line.Substring(pos).StartsWith("six"))
            {
                vals.Add(6);
                //pos += 2;
                continue;
            }
            if (line.Substring(pos).StartsWith("seven"))
            {
                vals.Add(7);
                //pos += 4;
                continue;
            }
            if (line.Substring(pos).StartsWith("eight"))
            {
                vals.Add(8);
                //pos += 4;
                continue;
            }
            if (line.Substring(pos).StartsWith("nine"))
            {
                vals.Add(9);
                //pos += 3;
                continue;
            }

        }

        int d = 0;
        if (vals.Count == 1)
        {
            d = (vals[0] * 10 + vals[0]);
        }
        else
        {
            d = (vals.First<int>() * 10) + vals.Last<int>();
        }
        total += d;
        Console.WriteLine($"{d} ==> {total} ___ {line}");
    }
    Console.WriteLine($"{total}");
}

static void Part1(string[] lines)
{
    long total = 0;
    foreach (var line in lines)
    {
        List<int> vals = new();
        foreach (char c in line)
        {
            if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
            {
                vals.Add((int)(c - '0'));
            }
        }
        if (vals.Count == 1)
        {
            total += (vals[0] * 10 + vals[0]);
        }
        else
        {
            total += (vals.First<int>() * 10) + vals.Last<int>();
        }
    }
    Console.WriteLine(total);
}
