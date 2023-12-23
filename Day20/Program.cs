// See https://aka.ms/new-console-template for more information

using System.Xml.Linq;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

//Part1(lines);
Part2(lines);

void Part1(string[] lines)
{

    Dictionary<string, Thing> things = new();
    Dictionary<string, List<string>> targets = new();

    foreach (var line in lines)
    {
        var spls = line.Split("->", StringSplitOptions.RemoveEmptyEntries);

        List<string> nodetargets = new();
        var targspls = spls[1].Split(',');
        foreach (var targs in targspls)
        {
            nodetargets.Add(targs.Trim());
        }

        string name;
        if (spls[0].StartsWith("broadcaster")) 
        {
            name = "broadcaster";
        }
        else
        {
            name = spls[0].Substring(1);            
        }
        name = name.Trim();
        targets.Add(name, nodetargets);

        if (spls[0].StartsWith("%"))
        {
            var slipslop = new FlipFlop() { name = name, targets = nodetargets };
            things.Add(name, slipslop);
        }
        if (spls[0].StartsWith("&"))
        {
            var conjun = new Conjunction() { name = name, targets = nodetargets };
            things.Add(name, conjun);
        }
    }

    foreach (var pair in things)
    {
        switch (pair.Value)
        {
            case Conjunction tcon :
                foreach (var pt in targets)
                {
                    foreach (var nm in pt.Value)
                    {
                        if (nm.CompareTo(tcon.name) == 0)
                        {
                            tcon.AddMemory(pt.Key);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    //
    int highsend = 0;
    int lowsend = 0;
    int buttons = 0;

    Queue<(string, string, Signal)> communications = new();
    for (int i = 0; i < 1000; i++)
    {
        buttons++;
        lowsend += 1;
        Console.WriteLine("==============================================================");
        communications.Enqueue(("", "broadcaster", Signal.Low));
        while (communications.Count > 0)
        {
            var comm = communications.Dequeue();
            (string from, string node, Signal sig) = comm;
            if (node.StartsWith("broadcaster"))
            {
                var btargs = targets["broadcaster"];
                foreach (var n in btargs)
                {
                    communications.Enqueue(("broadcaster", n, Signal.Low));
                    //Console.WriteLine($"broadcaster - Low - {n}");
                }
                //lowsend += btargs.Count;
            }
            else
            {
                if (things.ContainsKey(node))
                {
                    Console.WriteLine($"{from} - {sig} -> {node}");
                    var thing = things[node];
                    (int lv, int hv) = thing.Engage(from, sig, communications);
                    //highsend += hv;
                    //lowsend += lv;
                    if (sig == Signal.High)
                        highsend++;
                    else
                        lowsend++;
                }
                else
                {
                    Console.WriteLine($"{from} - {sig} -> {node}");
                    if (sig == Signal.High)
                        highsend++;
                    else
                        lowsend++;
                }
            }
        }
    }
    Console.WriteLine($"Lowsend = {lowsend}, Highsend = {highsend}");
    Console.WriteLine(lowsend * highsend);
}

void Part2(string[] lines)
{

    Dictionary<string, Thing> things = new();
    Dictionary<string, List<string>> targets = new();

    foreach (var line in lines)
    {
        var spls = line.Split("->", StringSplitOptions.RemoveEmptyEntries);

        List<string> nodetargets = new();
        var targspls = spls[1].Split(',');
        foreach (var targs in targspls)
        {
            nodetargets.Add(targs.Trim());
        }

        string name;
        if (spls[0].StartsWith("broadcaster"))
        {
            name = "broadcaster";
        }
        else
        {
            name = spls[0].Substring(1);
        }
        name = name.Trim();
        targets.Add(name, nodetargets);

        if (spls[0].StartsWith("%"))
        {
            var slipslop = new FlipFlop() { name = name, targets = nodetargets };
            things.Add(name, slipslop);
        }
        if (spls[0].StartsWith("&"))
        {
            var conjun = new Conjunction() { name = name, targets = nodetargets };
            things.Add(name, conjun);
        }
    }

    foreach (var pair in things)
    {
        switch (pair.Value)
        {
            case Conjunction tcon:
                foreach (var pt in targets)
                {
                    foreach (var nm in pt.Value)
                    {
                        if (nm.CompareTo(tcon.name) == 0)
                        {
                            tcon.AddMemory(pt.Key);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    //
    int highsend = 0;
    int lowsend = 0;
    long buttons = 0;

    Queue<(string, string, Signal)> communications = new();
    bool found = false;
    while(!found)
    {
        if (buttons % 1000000 == 0) Console.Write('.');
        buttons++;
        lowsend += 1;
        //Console.WriteLine("==============================================================");
        communications.Enqueue(("", "broadcaster", Signal.Low));
        while (communications.Count > 0)
        {
            var comm = communications.Dequeue();
            (string from, string node, Signal sig) = comm;
            if (node.StartsWith("broadcaster"))
            {
                var btargs = targets["broadcaster"];
                foreach (var n in btargs)
                {
                    communications.Enqueue(("broadcaster", n, Signal.Low));
                    //Console.WriteLine($"broadcaster - Low - {n}");
                }
                //lowsend += btargs.Count;
            }
            else
            {
                if (node.CompareTo("rx")==0 && sig == Signal.Low)
                {
                    found = true;
                    break;
                }
                if (things.ContainsKey(node))
                {
                    //Console.WriteLine($"{from} - {sig} -> {node}");
                    var thing = things[node];
                    (int lv, int hv) = thing.Engage(from, sig, communications);
                    //highsend += hv;
                    //lowsend += lv;
                    if (sig == Signal.High)
                        highsend++;
                    else
                        lowsend++;
                }
                else
                {
                    //Console.WriteLine($"{from} - {sig} -> {node}");
                    if (sig == Signal.High)
                        highsend++;
                    else
                        lowsend++;
                }
            }
        }
    }
    Console.WriteLine();
    Console.WriteLine($"Lowsend = {lowsend}, Highsend = {highsend}, Buttons = {buttons}");
    Console.WriteLine(lowsend * highsend);
}

public enum Signal { High = 1, Low = 2 }

public abstract class Thing 
{
    public string name { get; set; }
    public List<string> targets { get; set; }

    public abstract (int,int) Engage(string from, Signal pulse, Queue<(string, string, Signal)> communications);
}

public class FlipFlop : Thing
{
    public bool state { get; set; }

    public FlipFlop()
    {
        this.name = name;
        this.state = false;
    }

    public override (int,int) Engage(string from, Signal pulse, Queue<(string, string, Signal)> communications)
    {
        //Console.WriteLine($"{from} - {pulse} -> {name}");
        switch (pulse)
        {
            case Signal.Low:
                state = !state; // flipped.
                Signal toSend;
                if (state) // on
                {
                    toSend = Signal.High;
                }
                else
                {
                    toSend = Signal.Low;
                }
                foreach (var k in targets)
                {
                    communications.Enqueue((name, k, toSend));
                }
                if (toSend == Signal.High)
                    return (0, targets.Count);
                else
                    return (targets.Count, 0);

            case Signal.High:
                return (0,0); // just ignore.
        }
        return (0, 0);
    }
}

public class Conjunction : Thing
{
    public Dictionary<string, Signal> memory = new();
    public Conjunction()
    {
    }

    public void AddMemory(string n)
    {
        memory.Add(n, Signal.Low);
    }

    public override (int,int) Engage(string from, Signal pulse, Queue<(string, string, Signal)> communications)
    {

        //Console.WriteLine($"{from} - {pulse} -> {name}");

        Signal toSend = Signal.Low;
        memory[from] = pulse;

        foreach (var pair in memory)
        {
            if (pair.Value.Equals(Signal.Low))
            {
                toSend = Signal.High;
                break;
            }
        }
        foreach (var k in targets)
        {
            communications.Enqueue((name, k, toSend));
        }
        if (toSend != Signal.Low)
            return (targets.Count, 0);
        else
            return (0, targets.Count);
    }
}
