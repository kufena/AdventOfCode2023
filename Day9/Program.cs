// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    long total = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        long[] vals = new long[splits.Length];
        for (int i = 0; i < splits.Length; i++)
        {
            vals[i] = long.Parse(splits[i]);
        }
        var result = RecursiveFumble(vals, vals.Length);
        Console.WriteLine($"==================================================================");
        total += result;
    }
    Console.WriteLine(total);
}

static void Part2(string[] lines)
{
    long total = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        long[] vals = new long[splits.Length];
        for (int i = 0; i < splits.Length; i++)
        {
            vals[i] = long.Parse(splits[i]);
        }
        var result = RecursiveFumbleBackwards(vals, vals.Length);
        Console.WriteLine($"==================================================================");
        total += result;
    }
    Console.WriteLine(total);
}

static long RecursiveFumble(long[] arr, long len)
{
    for (int i = 0; i < len; i++)
    {
        Console.Write($"{arr[i]} ");
    }
    Console.WriteLine("");

    bool allzeros = true;
    for (int i = 0; i < len; i++)
    {
        if (arr[i] != 0)
        {
            allzeros = false;
            break;
        }
    }
    if (allzeros)
    {
        return 0;
    }

    long[] diffs = new long[len - 1];
    for (int i = 0; i < len - 1; i++)
    {
        diffs[i] = arr[i+1] - arr[i]; 
    }

    long recresult = RecursiveFumble(diffs, len - 1);
    long result = arr[len - 1] + recresult;

    for (int i = 0; i < len; i++)
    {
        Console.Write($"{arr[i]} ");
    }
    Console.WriteLine($"{result}");

    return result;
    
}

static long RecursiveFumbleBackwards(long[] arr, long len)
{
    for (int i = 0; i < len; i++)
    {
        Console.Write($"{arr[i]} ");
    }
    Console.WriteLine("");

    bool allzeros = true;
    for (int i = 0; i < len; i++)
    {
        if (arr[i] != 0)
        {
            allzeros = false;
            break;
        }
    }
    if (allzeros)
    {
        return 0;
    }

    long[] diffs = new long[len - 1];
    for (int i = 0; i < len - 1; i++)
    {
        diffs[i] = arr[i + 1] - arr[i];
    }

    long recresult = RecursiveFumbleBackwards(diffs, len - 1);
    long result = arr[0] - recresult;

    Console.Write($"{result}");
    for (int i = 0; i < len; i++)
    {
        Console.Write($" {arr[i]}");
    }
    Console.WriteLine("");
    return result;

}