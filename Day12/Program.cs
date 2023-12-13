// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
//Console.WriteLine($"True = {OurMatch("#.#..", new int[] { 1, 1 })}");
//Console.WriteLine($"False = {OurMatch("..##..", new int[] { 1, 1 })}");
//Console.WriteLine($"True = {OurMatch("#.##..", new int[] { 1, 2 })}");
//Console.WriteLine($"False = {OurMatch("..#.#..", new int[] { 1, 1,3 })}");

Part2(lines);

static void Part1(string[] lines)
{
    HashSet<char> comps = new HashSet<char>() { '?', '#', '.' };

    int count = 0;

    foreach (var line in lines)
    {
        Console.WriteLine(line);

        var splits = line.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        int[]? expected = splits[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
        if (expected is null) throw new Exception("What?");

        int required = expected.Sum();
        Console.WriteLine($"Expecting {required} hashes");

        string current = $"{splits[0][0]}";
        List<string> divs = new();
        for (int i = 1; i < splits[0].Length; i++)
        {
            if (splits[0][i] == current[current.Length - 1])
            {
                current += splits[0][i];
            }
            else
            {
                divs.Add(current);
                current = $"{splits[0][i]}";
            }
        }
        divs.Add(current);
        Console.WriteLine($"{divs.Count} divs");
        int hashcount = 0;
        int quescount = 0;
        foreach (var div in divs) 
        {
            if (div.StartsWith('#')) hashcount += div.Length;
            if (div.StartsWith('?')) quescount += div.Length;
            Console.Write($"{div} "); 
        }
        Console.WriteLine();

        Console.WriteLine($"{required} hashes from {quescount} question marks and {hashcount} existing, so {required - hashcount} dist.");
        // n! / k!(n-k)!
        long k = (long)(required - hashcount);
        long nfac = Factorial((long)quescount);
        long kfac = Factorial((long)k);
        long nminkfac = Factorial((long)(quescount - k));
        Console.WriteLine($"Dist  of {nfac / (kfac * nminkfac)}");

        var list = Distribute(quescount, (int)k, "");
        Console.WriteLine(list.Count);

        foreach (var l in list)
        {
            StringBuilder sb = new();
            int t = 0;
            foreach (var div in divs)
            {
                if (div.StartsWith("?"))
                {
                    sb.Append(l.Substring(t, div.Length));
                    t += div.Length;
                }
                else
                    sb.Append(div);
            }
            if (OurMatch(sb.ToString(), expected))
            {
                Console.WriteLine(sb.ToString());
                count += 1;
            }
        }

        Console.WriteLine("=============================================================================");
        Console.WriteLine();
        Console.WriteLine();

    }

    Console.WriteLine($"Total = {count}");
}

//
// This isn't going to work is it.
static void Part2(string[] lines)
{
    HashSet<char> comps = new HashSet<char>() { '?', '#', '.' };
    long[][] counts = new long[lines.Length][];
    for (int i = 0; i < lines.Length; i++) counts[i] = new long[6];
    for (long copies = 1; copies < 3; copies++)
    {
        Console.WriteLine($"{copies} copies now now now now now.");
        for(int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            Console.WriteLine(line);

            var splits = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb_spl0 = new();
            for (int hi = 0; hi < copies - 1; hi++)
            {
                sb_spl0.Append(splits[0]);
                sb_spl0.Append("?");
            }
            sb_spl0.Append(splits[0]);
            splits[0] = sb_spl0.ToString();

            StringBuilder sb_spl1 = new();
            for (int hi = 0; hi < copies - 1; hi++)
            {
                sb_spl1.Append(splits[1]);
                sb_spl1.Append(",");
            }
            sb_spl1.Append(splits[1]);
            splits[1] = sb_spl1.ToString();

            int[]? expected = splits[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
            if (expected is null) throw new Exception("What?");

            int required = expected.Sum();
            Console.WriteLine($"Expecting {required} hashes");

            string current = $"{splits[0][0]}";
            List<string> divs = new();
            for (int hi = 1; hi < splits[0].Length; hi++)
            {
                if (splits[0][hi] == current[current.Length - 1])
                {
                    current += splits[0][hi];
                }
                else
                {
                    divs.Add(current);
                    current = $"{splits[0][hi]}";
                }
            }
            divs.Add(current);
            Console.WriteLine($"{divs.Count} divs");
            int hashcount = 0;
            int quescount = 0;
            foreach (var div in divs)
            {
                if (div.StartsWith('#')) hashcount += div.Length;
                if (div.StartsWith('?')) quescount += div.Length;
                //Console.Write($"{div} ");
            }
            //Console.WriteLine();

            /*************************************
             * Hard Work Starts Here.
             * 
             * This works by doing 1 and then 2 copies, and
             * the result for R(n+2) is R(n+1)^2/R(n)
             * 
             *************************************/
            //Console.WriteLine($"{required} hashes from {quescount} question marks and {hashcount} existing, so {required - hashcount} dist.");
            // n! / k!(n-k)!
            long k = (long)(required - hashcount);
            //long nfac = Factorial((long)quescount);
            //long kfac = Factorial((long)k);
            //long nminkfac = Factorial((long)(quescount - k));
            //Console.WriteLine($"Dist  of {nfac / (kfac * nminkfac)}");

            var list = Distribute(quescount, k, "");
            Console.WriteLine($"{list.Count}");
            long x = 0;
            foreach (var l in list)
            {
                StringBuilder sb = new();
                int t = 0;
                foreach (var div in divs)
                {
                    if (div.StartsWith("?"))
                    {
                        sb.Append(l.Substring(t, div.Length));
                        t += div.Length;
                    }
                    else
                        sb.Append(div);
                }
                if (OurMatch(sb.ToString(), expected))
                {
                    //Console.WriteLine(sb.ToString());
                    x += 1;
                }
            }

            Console.WriteLine("=============================================================================");
            counts[i][copies] = x;
            Console.WriteLine(x);
        }
    }

    Console.WriteLine("+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+");
    Console.WriteLine("+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+");

    for (int copies = 3; copies < 6; copies++)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            counts[i][copies] = (counts[i][copies - 1] * counts[i][copies - 1]) / counts[i][copies - 2];

        }
    }

    long total = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        total += counts[i][5];
    }
    Console.WriteLine($"{total} is the number we want.");
}
static long Factorial(long l)
{
    long f = 1;
    for (int i = 2; i <= l; i++) f = f * i;
    return f;
}

static bool OurMatch(string line, int[] counts)
{
    int l = 0;
    int countgo = 0;
    int counttogo = 0;
    bool incount = false;
    int dots = 0;
    while (l < line.Length)
    {
        if (incount)
        {
            if (line[l] == '.') // end of count.
            {
                if (counttogo != 0) return false; // not enough hashes.
                if (counttogo == 0)
                {
                    incount = false;
                    dots = 1;
                }
            }
            else // it's a hash.
            {
                counttogo -= 1;
                if (counttogo < 0) return false; // we've overstepped the mark.
            }
        }
        else
        { // we're outside a count.
            if (line[l] == '.')
            {
                dots += 1;
            }
            else // it's a hash
            {
                if (countgo > 0 && dots == 0) return false; // at start we can have zero dots before but not subsequently.
                if (countgo >= counts.Length) return false; // no more hashes expected.
                counttogo = counts[countgo] - 1;
                countgo += 1;
                incount = true;
                dots = 0;
            }
        }
        l += 1;
    }
    if (countgo < counts.Length) return false; // not all matched.
    return true;
}

static List<string> Distribute(int quests, long hashes, string prefix)
{
    if (hashes == 0)
    {
        StringBuilder sb = new();
        for (int i = 0; i < quests; i++) sb.Append(".");
        return new List<string>() { prefix + sb.ToString() };
    }
    if (quests == 0)
        return new List<string>();

    if (quests == hashes)
    {
        StringBuilder sb = new();
        for (int i = 0; i < hashes; i++) sb.Append("#");
        return new List<string>() { prefix + sb.ToString() };
    }

    var l1 = Distribute(quests - 1, hashes - 1, prefix + "#");
    var l2 = Distribute(quests - 1, hashes, prefix + ".");
    foreach (var s in l2) l1.Add(s);
    return l1;
}