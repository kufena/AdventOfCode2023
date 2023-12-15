// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
//Part1(lines);
Part2(lines);

static void Part1(string[] lines)
{
    long total = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(',');
        foreach (var spl in splits)
        {
            long subtot = HashFunction(spl);
            total += subtot;
        }
    }
    Console.WriteLine($"Total = {total}");
}

static void Part2(string[] lines)
{
    Box[] boxes = new Box[256];
    for (int i = 0; i < boxes.Length; i++)
    {
        boxes[i] = new Box();
    }
    long total = 0;
    foreach (var line in lines)
    {
        var splits = line.Split(',');
        foreach (var spl in splits)
        {
            string[] subspls = spl.Split(new char[] { '-', '=' });
            int index = (int) HashFunction(subspls[0]);
            char instr = spl.Substring(subspls[0].Length, 1)[0];
            if (instr == '=') // add
            {
                int focallength = int.Parse(subspls[1]);
                boxes[index].AddLens(new Lens(subspls[0], focallength ));
            }
            else
            {
                boxes[index].RemoveLens(new Lens(subspls[0], -1 ));
            }
        }
    }
    for (int i = 0; i < 256; i++)
    {
        total += boxes[i].Sum(i+1);
    }
    Console.WriteLine($"Total = {total}");
}

static long HashFunction(string spl)
{
    long subtot = 0;
    var bytes = ASCIIEncoding.ASCII.GetBytes(spl);
    for (int i = 0; i < bytes.Length; i++)
    {
        subtot += ((long)bytes[i]);
        subtot = (subtot * 17) % 256;
    }
    Console.WriteLine($"Subtotal {spl} is {subtot}");
    return subtot;
}

public class Box
{
    HashSet<string> lensLabels;
    Dictionary<string, Lens> toLens;
    Lens top;
    Lens end;

    public Box()
    {
        toLens = new();
        lensLabels = new();
        top = null;
        end = null;
    }

    public long Sum(long boxnum)
    {
        if (lensLabels.Count == 0)
            return 0;
        var st = top;
        int count = 1;
        long tot = 0;
        while (st != null)
        {
            long v = boxnum * st.focalLength * count;
            Console.WriteLine($"Box {boxnum} {st.label} {v}");
            tot += v;
            st = st.next;
            count++;
        }
        Console.WriteLine($"Sum is {tot}");
        return tot;
    }

    public void AddLens(Lens l)
    {
        Console.WriteLine($"Adding {l.label} {l.focalLength}");
        if (lensLabels.Contains(l.label))
        {
            var old = toLens[l.label];
            l.back = old.back;
            l.next = old.next;
            if (old.back == null) // we're the start
            {
                top = l;
            }
            else
            {
                old.back.next = l;
            }
            if (old.next == null) // we're the end
            {
                end = l;
            }
            else
            {
                old.next.back = l;
            }
            toLens[l.label] = l;
        }
        else
        {
            lensLabels.Add(l.label);
            toLens.Add(l.label, l);
            if (end == null)
            {
                end = l;
                top = l;
            }
            else
            {
                end.next = l;
                l.back = end;
                end = l;
            }
        }
    }

    public void RemoveLens(Lens l)
    {
        Console.WriteLine($"Removing {l.label}");
        if (lensLabels.Contains(l.label))
        {
            lensLabels.Remove(l.label);
            var old = toLens[l.label];
            toLens.Remove(l.label);
            if (old.back == null)
            {
                top = old.next;
            }
            else
            {
                old.back.next = old.next;
            }
            if (old.next == null)
            {
                end = old.back;
            }
            else
            {
                old.next.back = old.back;
            }
        }
    }
}

public class Lens
{
    public string label { get; set; }
    public int focalLength { get; set; }
    public Lens next { get; set; }
    public Lens back { get; set; }
    
    public Lens(string l, int f)
    {
        label = l;
        focalLength = f;
        next = null;
        back = null;
    }
}