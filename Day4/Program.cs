// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    long total = 0;
    bool[] arr = new bool[100];

    foreach (var line in lines)
    {
        var wincard = line.Split('|');
        //foreach(var s in wincard) Console.WriteLine(s);
        var winsplit = wincard[0].Split(':');
        var winnums = winsplit[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //foreach(var s in winnums) Console.WriteLine(s);
        var winnings = new int[winnums.Length];
        for (int i = 0; i < winnums.Length; i++)
        {
            winnings[i] = Int32.Parse(winnums[i]);
            arr[winnings[i]] = true;
        }

        // check our nums.
        int count = 0;
        var carsplit = wincard[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var carnum in carsplit)
        {
            var x = int.Parse(carnum);
            if (arr[x]) count++;
        }
        if (count > 0)
        {
            Console.WriteLine($"{winsplit[0]}: {count} matches adding {Math.Pow(2,count-1)}");
            total += (long)(Math.Pow(2,count-1));
        }
        // reset arr for next line
        for (int i = 0; i < winnums.Length; i++) arr[winnings[i]] = false;
    }

    Console.WriteLine(total);
}
static void Part2(string[] lines)
{
    long total = 0;
    bool[] arr = new bool[100];
    long[] wincounts = new long[lines.Length];
    Stack<int> cards = new Stack<int>();

    for(int q = 0; q < lines.Length; q++)
    {
        var line = lines[q];
        var wincard = line.Split('|');
        //foreach(var s in wincard) Console.WriteLine(s);
        var winsplit = wincard[0].Split(':');
        var winnums = winsplit[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //foreach(var s in winnums) Console.WriteLine(s);
        var winnings = new int[winnums.Length];
        for (int i = 0; i < winnums.Length; i++)
        {
            winnings[i] = Int32.Parse(winnums[i]);
            arr[winnings[i]] = true;
        }

        // check our nums.
        int count = 0;
        var carsplit = wincard[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var carnum in carsplit)
        {
            var x = int.Parse(carnum);
            if (arr[x]) count++;
        }
        wincounts[q] = count;
        cards.Push(q);
        // reset arr for next line
        for (int i = 0; i < winnums.Length; i++) arr[winnings[i]] = false;
    }

    while (cards.Count > 0)
    {
        total += 1;
        int ne = cards.Pop();
        if (wincounts[ne] > 0)
        {
            for (int j = 0; j < wincounts[ne]; j++)
                cards.Push(ne + j + 1);
        }
    }
    Console.WriteLine(total);
}
