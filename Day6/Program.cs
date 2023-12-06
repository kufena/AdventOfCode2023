// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    var timl1 = lines[0].Split(':');
    var timl2 = timl1[1].Split(' ',StringSplitOptions.RemoveEmptyEntries);
    int[] times = new int[timl2.Length];
    for (int i = 0; i < times.Length; i++)
        times[i] = int.Parse(timl2[i]);
    var disl1 = lines[1].Split(':');
    var disl2 = disl1[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    int[] distances = new int[disl2.Length];
    for (int i = 0; i < distances.Length; i++)
        distances[i] = int.Parse(disl2[i]);

    int[] counts = new int[distances.Length];
    for (int i = 0; i < counts.Length; i++)
    {
        for (int j = 0; j < times[i]; j++)
        {
            if ((times[i] * j) - (j * j) > distances[i])
            {
                counts[i] = j;
                break;
            }
        }
        Console.WriteLine($"{times[i]} {distances[i]} = {counts[i]}");

        for (int j = times[i] - 1; j >= 0; j--)
        {
            if ((times[i] * j) - (j * j) > distances[i])
            {
                counts[i] = (j - counts[i]) + 1;
                break;
            }
        }
        Console.WriteLine($"{times[i]} {distances[i]} = {counts[i]}");
    }
    int total = 1;
    for (int i = 0; i < counts.Length; i++)
    {
        total = total * counts[i];
    }
    Console.WriteLine(total);
}
static void Part2(string[] lines)
{
    var timl1 = lines[0].Split(':');
    var timl2 = timl1[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    string[] timl3 = new string[] { String.Concat(timl2) };
    long[] times = new long[timl3.Length];
    for (int i = 0; i < times.Length; i++)
        times[i] = long.Parse(timl3[i]);
    var disl1 = lines[1].Split(':');
    var disl2 = disl1[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    string[] disl3 = new string[] { String.Concat(disl2) };
    long[] distances = new long[disl3.Length];
    for (int i = 0; i < distances.Length; i++)
        distances[i] = long.Parse(disl3[i]);

    long[] counts = new long[distances.Length];
    for (int i = 0; i < counts.Length; i++)
    {
        for (long j = 0; j < times[i]; j++)
        {
            if ((times[i] * j) - (j * j) > distances[i])
            {
                counts[i] = j;
                break;
            }
        }
        Console.WriteLine($"{times[i]} {distances[i]} = {counts[i]}");

        for (long j = times[i] - 1; j >= 0; j--)
        {
            if ((times[i] * j) - (j * j) > distances[i])
            {
                counts[i] = (j - counts[i]) + 1;
                break;
            }
        }
        Console.WriteLine($"{times[i]} {distances[i]} = {counts[i]}");
    }
    long total = 1;
    for (int i = 0; i < counts.Length; i++)
    {
        total = total * counts[i];
    }
    Console.WriteLine(total);
}