// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Formats.Asn1;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
//Part1(lines);
Part2(lines);
//Part2Brute(lines);

void Part1(string[] lines)
{
    Dictionary<string, List<Rule>> rules = new();
    int i = 0;
    while (lines[i].Trim() != "")
    {
        var spl = lines[i].Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        rules.Add(spl[0], ParseRules(spl[1]));
        i++;
    }
    i++;
    Stack<(Data, string)> doStack = new();
    for (int j = i; j < lines.Length; j++)
    {
        var spls = lines[j].Split(new char[] { 'x', 'm', 'a', 's', ',', '{', '}', '=' }, StringSplitOptions.RemoveEmptyEntries);
        var rec = new Data(long.Parse(spls[0]), long.Parse(spls[1]), long.Parse(spls[2]), long.Parse(spls[3]));
        doStack.Push((rec, "in"));
    }

    List<Data> accepted = LoopOverRules(rules, doStack);

    Console.WriteLine("Doing sums.");
    long total = 0;
    foreach (var n in accepted)
    {
        total += (n.x + n.m + n.a + n.s);
    }
    Console.WriteLine(total);
}

static void Part2Brute(string[] lines)
{
    Dictionary<string, List<Rule>> rules = new();
    int i = 0;
    while (lines[i].Trim() != "")
    {
        var spl = lines[i].Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        rules.Add(spl[0], ParseRules(spl[1]));
        i++;
    }

    List<long> xboundaries = new();
    List<long> mboundaries = new();
    List<long> aboundaries = new();
    List<long> sboundaries = new();

    foreach (var p in rules)
    {
        foreach (var r in p.Value)
        {
            if (r is ConstraintRule)
            {
                var cr = (ConstraintRule)r;
                if (cr.xmas.Equals("x"))
                {
                    xboundaries.Add(cr.val - 1);
                    xboundaries.Add(cr.val);
                    xboundaries.Add(cr.val + 1);
                }
                if (cr.xmas.Equals("m"))
                {
                    mboundaries.Add(cr.val - 1);
                    mboundaries.Add(cr.val);
                    mboundaries.Add(cr.val + 1);
                }
                if (cr.xmas.Equals("a"))
                {
                    aboundaries.Add(cr.val - 1);
                    aboundaries.Add(cr.val);
                    aboundaries.Add(cr.val + 1);
                }
                if (cr.xmas.Equals("s"))
                {
                    sboundaries.Add(cr.val - 1);
                    sboundaries.Add(cr.val);
                    sboundaries.Add(cr.val + 1);
                }

            }
        }
    }

    i++;
    long total = 0;
    foreach (long x in xboundaries)
    {
        Console.Write('.');
        foreach (long m in mboundaries)
        {
            Console.Write($"+({total})");
            Stack<(Data, string)> doStack = new();
            foreach (long a in aboundaries)
            {
                foreach (long s in sboundaries)
                {
                    var rec = new Data(x, m, a, s);
                    doStack.Push((rec,"in"));
                }
            }
            List<Data> accepted = LoopOverRules(rules, doStack);
            total += accepted.Count;
        }
    }
    Console.WriteLine();
    Console.WriteLine(total);
}


// Oh, I wish I'd done the graph originally.
static void Part2(string[] lines)
{
    Dictionary<string, Node> nodes = new();
    Dictionary<string, List<Edge>> edges = new();

    Node accept = new Node() { name = "accept" };
    Node reject = new Node() { name = "reject" };

    nodes.Add("accept", accept);
    nodes.Add("reject", reject);

    int i = 0;
    while (lines[i].Trim() != "")
    {
        var spl = lines[i].Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        var nodename = spl[0];
        Node n;
        if (nodes.ContainsKey(nodename))
        {
            n = nodes[nodename];
        }
        else
        {
            n = new Node() { name = nodename };
            nodes.Add(nodename, n);
        }
        List<Edge> nodeedges;
        if (edges.ContainsKey(n.name)) {
            nodeedges = edges[n.name];
        }
        else 
        {
            nodeedges = new();
            edges.Add(n.name, nodeedges);
        }
        var noderules = ParseRules(spl[1]);
        foreach (var rule in noderules)
        {
            switch (rule) 
            {
                case AcceptRule:
                    nodeedges.Add(new Edge() { start = n, end = accept, rule = AcceptRule.Accepter });
                    break;
                case RejectRule:
                    nodeedges.Add(new Edge() { start = n, end = reject, rule = AcceptRule.Accepter });
                    break;
                case StateRule s:
                    Node target;
                    if (nodes.ContainsKey(s.destination.state))
                    {
                        target = nodes[s.destination.state];
                    }
                    else 
                    {
                        target = new Node() { name = s.destination.state };
                        nodes.Add(s.destination.state, target);
                    }
                    nodeedges.Add(new Edge() { start = n, end = target, rule = s });
                    break;
                case ConstraintRule c:
                    Node ctarget;
                    switch (c.destination)
                    {
                        case Accept:
                            ctarget = accept;
                            break;
                        case Reject:
                            ctarget = reject;
                            break;
                        case State st:
                            if (nodes.ContainsKey(st.state))
                            {
                                ctarget = nodes[st.state];
                            }
                            else
                            {
                                ctarget = new Node() { name = st.state };
                                nodes.Add(st.state, ctarget);
                            }
                            break;
                        default:
                            throw new Exception("unexpected state or lack of in ConstraintRule");
                    }
                    nodeedges.Add(new Edge() { start = n, end = ctarget, rule = c });
                    break;
            }
        }
        i++;
    }

    Console.WriteLine("What have we done?");
    var totAccept = FindAllPaths("in", "accept", nodes, edges);
    var totReject = FindAllPaths("in", "reject", nodes, edges);

    Console.WriteLine($"Accept = {totAccept.Count}, Reject = {totReject.Count}");

    long total = 0;
    long most = 4000L * 4000L * 4000L * 4000L;

    List<Ranger> rangers = new();
    foreach (var ll in totAccept)
//    foreach (var ll in totReject)
        {
            long xmin = 0;
        long xmax = 4000;
        long mmin = 0;
        long mmax = 4000;
        long amin = 0;
        long amax = 4000;
        long smin = 0;
        long smax = 4000;
        ll.Reverse();
        foreach (var e in ll)
        {
            Console.Write($"{e.start.name}");
            switch (e.rule)
            {
                case ConstraintRule c:
                    Console.Write($"({c.xmas}");
                    if (c.lessOrMore) 
                        Console.Write('<');
                    else 
                        Console.Write('>');
                    Console.Write($"{c.val})");

                    if (c.lessOrMore) // < c.val
                    {
                        if (c.xmas.StartsWith("x")) { if (c.val < xmax) xmax = c.val; }
                        if (c.xmas.StartsWith("m")) { if (c.val < mmax) mmax = c.val; }
                        if (c.xmas.StartsWith("a")) { if (c.val < amax) amax = c.val; }
                        if (c.xmas.StartsWith("s")) { if (c.val < smax) smax = c.val; }
                    }
                    else // > c.val
                    {
                        if (c.xmas.StartsWith("x")) { if (c.val > xmin) xmin = c.val; }
                        if (c.xmas.StartsWith("m")) { if (c.val > mmin) mmin = c.val; }
                        if (c.xmas.StartsWith("a")) { if (c.val > amin) amin = c.val; }
                        if (c.xmas.StartsWith("s")) { if (c.val > smin) smin = c.val; }
                    }
                    break;
                case StateRule s:
                    Console.Write("(all)");
                    break;
                default:
                    break;
            }
            Console.Write("-");
        }
        long cheechee = ((xmax - xmin) * (mmax - mmin) * (amax - amin) * (smax - smin));
        Console.Write($"{ll.Last().end.name} (x:{xmin}to{xmax} m:{mmin}to{mmax} a:{amin}to{amax} s:{smin}to{smax})");
        Console.WriteLine();
        Ranger r = new Ranger(xmin, xmax, mmin, mmax, amin, amax, smin, smax);
        rangers.Add(r);
    }

    bool change = true;
    while (change)
    {
        change = false;
        var allrangers = rangers.ToArray();
        List<Ranger> toremove = new();
        for (int fixi = 0; fixi < allrangers.Length; fixi++)
        {
            for (int j = fixi + 1; j < allrangers.Length; j++)
            {
                // if they're contained then ok, we can remove a range.
                if (allrangers[j].mmin >= allrangers[fixi].mmin &&
                    allrangers[j].mmax <= allrangers[fixi].mmax &&
                    allrangers[j].xmin >= allrangers[fixi].xmin &&
                    allrangers[j].xmax <= allrangers[fixi].xmax &&
                    allrangers[j].amin >= allrangers[fixi].amin &&
                    allrangers[j].amax <= allrangers[fixi].amax &&
                    allrangers[j].smin >= allrangers[fixi].smin &&
                    allrangers[j].smax <= allrangers[fixi].smax)
                {
                    toremove.Add(allrangers[j]);
                    change = true;
                    break;
                }
                if (allrangers[fixi].mmin >= allrangers[j].mmin &&
                    allrangers[fixi].mmax <= allrangers[j].mmax &&
                    allrangers[fixi].xmin >= allrangers[j].xmin &&
                    allrangers[fixi].xmax <= allrangers[j].xmax &&
                    allrangers[fixi].amin >= allrangers[j].amin &&
                    allrangers[fixi].amax <= allrangers[j].amax &&
                    allrangers[fixi].smin >= allrangers[j].smin &&
                    allrangers[fixi].smax <= allrangers[j].smax)
                {
                    toremove.Add(allrangers[fixi]);
                    change = true;
                    break;
                }
            }
        }
        if (change)
        {
            foreach (Ranger ranger in toremove) rangers.Remove(ranger);
        }
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("Remaining ranges are:");
    foreach (var r in rangers)
    {
        long nsubtot = (r.xmax - r.xmin) * (r.mmax - r.mmin) * (r.amax - r.amin) * (r.smax - r.smin);
        Console.WriteLine($"x:{r.xmin}to{r.xmax} m:{r.mmin}to{r.mmax} a:{r.amin}to{r.amax} s:{r.smin}to{r.smax}  {nsubtot}");
        total += nsubtot;
    }
    Console.WriteLine(total);

    Console.WriteLine(most - total);

    long newtot = 0;
    HashSet<(int, int, int, int)> allTogetherNow = new();
    foreach (var r in rangers)
    {
        Console.WriteLine("Rrrranger..");
        for (int x = (int)r.xmin; x < r.xmax; x++)
        {
            for (int m = (int)r.mmin; m < r.mmax; m++)
            {
                for (int a = (int)r.amin; a < r.amax; a++)
                {
                    for (int s = (int)r.smin; s < r.smax; s++)
                    {
                        allTogetherNow.Add((x, m, a, s));
                    }
                }
            }
        }
    }
    Console.WriteLine(allTogetherNow.Count);
}

// we're kind of assuming 'accept' and 'reject' are the end of everything, and there are no loops.
static List<List<Edge>> FindAllPaths(string start, string end, Dictionary<string,Node> nodes, Dictionary<string, List<Edge>> edges)
{
    var returns = new List<List<Edge>>();

    if (!edges.ContainsKey(start))
        return new List<List<Edge>>() { };

    var myedges = edges[start];
    foreach (var edge in myedges)
    {
        if (edge.end.name == end)
        {
            returns.Add(new List<Edge>() { edge });
        }
        else
        {
            var subs = FindAllPaths(edge.end.name, end, nodes, edges);
            foreach (var sub in subs) // it's a flat map kid of situation.
            {
                sub.Add(edge);
                returns.Add(sub);
            }
        }
    }
    return returns;
}
static List<Rule> ParseRules(string rule)
{
    var indivs = rule.Split(',', StringSplitOptions.RemoveEmptyEntries);
    List<Rule> rules = new();
    for (int i = 0; i < indivs.Length; i++)
    {
        if (indivs[i].Equals("A"))
            rules.Add(AcceptRule.Accepter);
        else if (indivs[i].Equals("R"))
            rules.Add(RejectRule.Rejecter);
        else
        {
            if (indivs[i].Contains(":"))
            {
                var con = new ConstraintRule(indivs[i]);
                rules.Add(con);
            }
            else
            {
                rules.Add(new StateRule(indivs[i]));
            }
        }
    }
    return rules;
}


static List<Data> LoopOverRules(Dictionary<string, List<Rule>> rules, Stack<(Data, string)> doStack)
{
    List<Data> accepted = new();
    while (doStack.Count > 0)
    {
        var rec = doStack.Pop();
        (Data d, string rule) = rec;
        var rulelist = rules[rule];
        bool doNext = true;
        foreach (var apprule in rulelist)
        {
            var res = apprule.Apply(d);
            switch (res)
            {
                case None:
                    continue;
                case Accept:
                    accepted.Add(d);
                    doNext = false;
                    break;
                case Reject:
                    doNext = false;
                    break;
                case State s:
                    doNext = false;
                    doStack.Push((d, s.state));
                    break;
            }
            if (!doNext) break;
        }
    }

    return accepted;
}

/// <summary>
/// ///////////////////////////////////////////////////////////////////
/// </summary>
public class Node
{
    public string name { get; set; }

}

public class Edge
{
    public Node start { get; set; }
    public Node end { get; set; }

    public Rule rule { get; set; }
}

public record Data(long x, long m, long a, long s);

public abstract class Rule
{
    public abstract Destination Apply(Data data);
}

public class AcceptRule : Rule
{
    public static AcceptRule Accepter = new AcceptRule();
    public override Destination Apply(Data data)
    {
        return Accept.AcceptDest;
    }
}
public class RejectRule : Rule
{
    public static RejectRule Rejecter = new RejectRule();
    public override Destination Apply(Data data)
    {
        return Reject.RejectDest;
    }
}

public class StateRule : Rule
{
    public State destination { get; init; }
    public StateRule(string st)
    {
        destination = new State() { state = st };
    }

    public override Destination Apply(Data data)
    {
        return destination;
    }
}

public class ConstraintRule : Rule
{
    public bool lessOrMore { get; init; } // false => <, true => >
    public long val { get; init; }
    public string xmas { get; init; }
    public Destination destination { get; set; }

    public ConstraintRule(string rule)
    {
        var splits = rule.Split(':');
        if (splits[1].Equals("A"))
        {
            destination = Accept.AcceptDest;
        }
        else if (splits[1].Equals("R"))
        {
            destination = Reject.RejectDest;
        }
        else
        {
            destination = new State() { state = splits[1] };
        }
        lessOrMore = (splits[0].Contains("<")) ;

        var consplits = splits[0].Split(new char[] { '<', '>' });
        xmas = consplits[0];
        val = long.Parse(consplits[1]);
    }

    public override Destination Apply(Data data)
    {
        long comp = 1;
        if (xmas.Equals("x")) comp = data.x;
        if (xmas.Equals("m")) comp = data.m;
        if (xmas.Equals("a")) comp = data.a;
        if (xmas.Equals("s")) comp = data.s;

        bool truth = false;
        if (lessOrMore)
            truth = (comp < val);
        else
            truth = (comp > val);

        if (truth)
            return destination;
        else
        {
            return None.NoneDest;
        }
    }
}

//
// Destinations.
public abstract class Destination
{
    
}

public class None : Destination
{
    public static None NoneDest = new None();
}

public class Accept : Destination 
{
    public static Accept AcceptDest = new Accept();
}
public class Reject :  Destination 
{
    public static Reject RejectDest = new Reject();
}

public class State : Destination 
{ 
    public string state { get; set; }
}

public record Ranger(long xmin, long xmax, long mmin, long mmax, long amin, long amax, long smin, long smax);
