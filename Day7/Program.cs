// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    PART1Handy[] hands = new PART1Handy[lines.Length];
    Dictionary<long, List<PART1Handy>> rankedHands = new();
    for (int i = 0; i < lines.Length; i++)
    {
        hands[i] = PART1Handy.Parse(lines[i]);
    }
    Array.Sort(hands);
    long total = 0;
    for (int i = 0; i < hands.Length; i++)
    {
        Console.WriteLine($"{hands[i].Hand} has rank {i + 1}");
        total += (i + 1) * hands[i].Score;
    }
    Console.WriteLine(total);
}

static void Part2(string[] lines)
{
    PART2Handy[] hands = new PART2Handy[lines.Length];
    Dictionary<long, List<PART2Handy>> rankedHands = new();
    for (int i = 0; i < lines.Length; i++)
    {
        hands[i] = PART2Handy.Parse(lines[i]);
        long r = hands[i].Rank();
        if (rankedHands.ContainsKey(r))
            rankedHands[r].Add(hands[i]);
        else
            rankedHands.Add(r, new List<PART2Handy>() { hands[i] });
    }

    long total = 0;
    int cc = 1;

    var keyarr = rankedHands.Keys.ToArray();
    Array.Sort(keyarr);
    for (int j = 0; j < keyarr.Length; j++) 
    {
        var allhandsatrank = rankedHands[keyarr[j]].ToArray();
        Array.Sort(allhandsatrank);
        for (int k = 0; k < allhandsatrank.Length; k++)
        {
            Console.WriteLine($"{allhandsatrank[k].Hand} ({keyarr[j]}) has rank {cc}");
            total += cc * allhandsatrank[k].Score;
            cc++;
        }
    }
    Console.WriteLine(total);
}

public class PART2Handy : IComparable<PART2Handy>
{
    public string Hand { get; set; }
    public long Score { get; set; }

    public static PART2Handy Parse(string l)
    {
        var parts = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new PART2Handy
        {
            Hand = parts[0],
            Score = long.Parse(parts[1]),
        };
    }

    public long Rank()
    {
        Dictionary<char, int> counts = new();
        int jacks = 0;
        for (int i = 0; i < 5; i++)
        {
            char c = Hand[i];
            if (c == 'J')
                jacks += 1;
            else
            {
                if (counts.ContainsKey(c))
                    counts[c] += 1;
                else
                    counts.Add(c, 1);
            }
        }

        if (jacks == 5) // it's automagically a tip top hand
            return 6;

        if (jacks > 0)
        {
            Console.Write($"{Hand} has {jacks} jacks, ranked to ");
            // add jacks to highest or one of the highest cards.
            Dictionary<int, List<char>> backs = new();
            foreach (char c in counts.Keys)
            {
                if (backs.ContainsKey(counts[c]))
                    backs[counts[c]].Add(c);
                else
                    backs.Add(counts[c], new List<char>() { c });
            }
            int[] bcs = backs.Keys.ToArray();
            Array.Sort(bcs);
            int highcount = bcs[bcs.Length - 1];
            var cardvs = backs[highcount].ToArray();
            Array.Sort(cardvs, new PART2IComparer());
            counts[cardvs[cardvs.Length-1]] += jacks;
        }

        long finalrank = -1;
        if (counts.Keys.Count == 5) finalrank = 0; // High Card
        if (counts.Keys.Count == 4) finalrank = 1; // One Pair
        if (counts.Keys.Count == 3)
        {
            // Two Pair
            // Three of a kind
            foreach (var k in counts.Values)
            {
                if (k == 3) { finalrank = 3; break; }
            }
            if (finalrank < 0)
                finalrank = 2;
        }
        if (counts.Keys.Count == 2)
        {
            // Full House
            // Four of a kind
            foreach (var k in counts.Values)
            {
                if (k == 4) { finalrank = 5; break; }
            }
            if (finalrank < 0) finalrank = 4;
        }
        if (counts.Keys.Count == 1) finalrank = 6;
        if (jacks > 0) Console.WriteLine(finalrank);
        return finalrank;
//        throw new Exception("too many cards in hand.");
    }

    public int map(char c)
    {
        if (c == 'A') return 14;
        if (c == 'K') return 13;
        if (c == 'Q') return 12;
        if (c == 'J') return 0;
        if (c == 'T') return 11;
        return (c - '0') + 1;
    }

    public int CompareTo(PART2Handy? other)
    {
        if (other == null) throw new Exception("Uncomparable if null");
//        long myrank = this.Rank();
//        long theirrank = other.Rank();

//        if (myrank > theirrank) return 1;
//        if (myrank < theirrank) return -1;

        // same rank - order by cards.
        for (int i = 0; i < 5; i++)
        {
            int me = map(Hand[i]);
            int them = map(other.Hand[i]);
            if (me < them) return -1;
            if (me > them) return 1;
        }

        return 0;
    }
}

public class PART2IComparer : IComparer<char>
{
    public int Compare(char x, char y)
    {
        var xa = map(x);
        var ya = map(y);
        if (xa < ya) return -1;
        if (ya < xa) return 1;
        return 0;
    }

    // yes I know this is a repeat, but hey ho.
    public int map(char c)
    {
        if (c == 'A') return 14;
        if (c == 'K') return 13;
        if (c == 'Q') return 12;
        if (c == 'J') return 0;
        if (c == 'T') return 11;
        return (c - '0') + 1;
    }
}

public class PART1Handy : IComparable<PART1Handy>
{
    public string Hand { get; set; }
    public long Score { get; set; }

    public static PART1Handy Parse(string l)
    {
        var parts = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new PART1Handy
        {
            Hand = parts[0],
            Score = long.Parse(parts[1]),
        };
    }

    public long Rank()
    {
        Dictionary<char, int> counts = new();
        for (int i = 0; i < 5; i++)
        {
            char c = Hand[i];
            if (counts.ContainsKey(c))
                counts[c] += 1;
            else
                counts.Add(c, 1);
        }
        if (counts.Keys.Count == 5) return 0; // High Card
        if (counts.Keys.Count == 4) return 1; // One Pair
        if (counts.Keys.Count == 3)
        {
            // Two Pair
            // Three of a kind
            foreach (var k in counts.Values)
            {
                if (k == 3) return 3;
            }
            return 2;
        }
        if (counts.Keys.Count == 2)
        {
            // Full House
            // Four of a kind
            foreach (var k in counts.Values)
            {
                if (k == 4) return 5;
            }
            return 4;
        }
        if (counts.Keys.Count == 1) return 6;
        throw new Exception("too many cards in hand.");
    }

    public int map(char c)
    {
        if (c == 'A') return 14;
        if (c == 'K') return 13;
        if (c == 'Q') return 12;
        if (c == 'J') return 11;
        if (c == 'T') return 10;
        return c - '0';
    }

    public int CompareTo(PART1Handy? other)
    {
        if (other == null) throw new Exception("Uncomparable if null");
        long myrank = this.Rank();
        long theirrank = other.Rank();

        if (myrank > theirrank) return 1;
        if (myrank < theirrank) return -1;

        // same rank - order by cards.
        for (int i = 0; i < 5; i++)
        {
            int me = map(Hand[i]);
            int them = map(other.Hand[i]);
            if (me < them) return -1;
            if (me > them) return 1;
        }

        return 0;
    }
}
