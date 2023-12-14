// See https://aka.ms/new-console-template for more information
using System.ComponentModel;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//var l = Transpose(new string[] { "Andy", "Paul", "Pete", "Jane", "Toad", "grOT" }, 1, 6);
//foreach (var s in l) Console.WriteLine(s);

Part1(lines);

static void Part1(string[] lines)
{
    int total = 0;
    int prev = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        if (lines[i].Trim().Length == 0 || i == lines.Length - 1)
        {
            // we've got a grouping, go!
            int endLine = i == lines.Length - 1 ? i + 1 : i;
            for (int k = prev; k < endLine; k++)
            {
                Console.WriteLine(lines[k]);
            }
            var linesT = Transpose(lines, prev, endLine);
            (int byrow, int rowcount) = Chums(lines, prev, endLine);
            (int bycol, int colcount) = Chums(linesT, 0, linesT.Length);
            if (byrow == -1 && bycol == -1)
            {
                Console.WriteLine("No reflecting here.");
                continue; // no reflection.
            }
            if (byrow == bycol) Console.WriteLine("£££££££££££££££££££££££££££££££££££££££££££££££££££££££££");
            if (byrow > 0)
                total += (100 * (rowcount));
            else if (bycol > 0)
                total += colcount;
            Console.WriteLine($"byrow = {byrow},{rowcount} and bycol = {bycol},{colcount}");
            // do start of next.
            prev = i + 1;
            Console.WriteLine("Enter");
            //Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
    Console.WriteLine($"Total = {total}");
}

static string[] Transpose(string[] lines, int start, int end)
{
    string[] strs = new string[lines[start].Length];
    for (int i = start; i < end; i++)
    {
        for (int j = 0; j < lines[start].Length; j++)
        {
            strs[j] += lines[i][j];
        }
    }
    return strs;
}

static (int,int) Chums(string[] lines, int start, int end)
{
    int largest = -1;
    int rowsabove = -1;
    for (int i = start; i < end-1; i++)
    {
        if (lines[i] == lines[i + 1])
        {
            // we've found a potential reflection point.
            int count = 1;
            int nrowsabove = i - start;
            int mstart = i;
            int mend = i+1;
            for (int j = 1; (i-j >= start) && (i+1+j < end) ; j++)
            {
                if (lines[i - j] == lines[i + 1 + j])
                {
                    count += 1;
                    mstart = i - j;
                    mend = i + 1 + j;
                }
                else
                {
                    break;
                }
            }
            if (mstart == start || mend == end-1)
            {
                // We need to think of those imaginary matches.
                if (count > largest)
                {
                    largest = count;
                    rowsabove = nrowsabove;
                }
            }
        }

    }
    return (largest,rowsabove+1);
}