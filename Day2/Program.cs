// See https://aka.ms/new-console-template for more information
using System.ComponentModel.Design;
using System.Numerics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    const int red = 12;
    const int green = 13;
    const int blue = 14;

    int total = 0;

    foreach (var line in lines)
    {
        var fsplits = line.Split(':');
        int id = Int32.Parse(fsplits[0].Substring(4));

        var picksplits = fsplits[1].Split(";");
        bool ok = true;

        foreach (var pick in picksplits)
        {
            var ballsplits = pick.Split(",");
            int redc = 0;
            int bluec = 0;
            int greenc = 0;

            foreach (var ball in ballsplits)
            {
                var ballexsplits = ball.Split(" ",StringSplitOptions.RemoveEmptyEntries);
                int ballval = Int32.Parse(ballexsplits[0]);
                if (ballexsplits[1].EndsWith("red")) redc = ballval;
                if (ballexsplits[1].EndsWith("blue")) bluec = ballval;
                if (ballexsplits[1].EndsWith("green")) greenc = ballval;
            }

            if (!(redc <= red && bluec <= blue && greenc <= green))
            {
                ok = false;
                break;
            }
        }
        if (ok) total += id;
    }
    Console.WriteLine(total);
}

static void Part2(string[] lines)
{
    long total = 0;

    foreach (var line in lines)
    {
        var fsplits = line.Split(':');
        int id = Int32.Parse(fsplits[0].Substring(4));

        var picksplits = fsplits[1].Split(";");

        int highred = 0;
        int highgreen = 0;
        int highblue = 0;

        foreach (var pick in picksplits)
        {
            var ballsplits = pick.Split(",");

            foreach (var ball in ballsplits)
            {
                var ballexsplits = ball.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                int ballval = Int32.Parse(ballexsplits[0]);
                if (ballexsplits[1].EndsWith("red")) if (ballval > highred) highred = ballval;
                if (ballexsplits[1].EndsWith("blue")) if (ballval > highblue) highblue = ballval;
                if (ballexsplits[1].EndsWith("green")) if (ballval > highgreen) highgreen = ballval;
            }
        }
        total += (highred * highblue * highgreen);
    }
    Console.WriteLine(total);
}

