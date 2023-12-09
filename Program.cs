using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

List<string> lines = new();
using (StreamReader reader = new(args[0]))
{
    while (!reader.EndOfStream)
    {
        lines.Add(reader.ReadLine());
    }
}

solve("AAA", "ZZZ");    //part1
solve(".??A", ".??Z");  //part2

void solve(string starts, string ends)
{
    string RL = lines[0];
    Dictionary<string, Node> nodes = new();

    //key=starting position, value=endHits to detect periods and initial offset
    Dictionary<string, List<EndHit>> startingPositions = new();
    foreach (string line in lines.Skip(2))
    {
        var split1 = line.Split(" = ");
        var split2 = split1[1].Split(", ");

        string curr = split1[0];
        string L = split2[0].Replace("(", "");
        string R = split2[1].Replace(")", "");

        nodes.Add(curr, new Node(L, R));

        if (Regex.IsMatch(curr, starts))
            startingPositions.Add(curr, new());
    }

    //Console.WriteLine($"tape period:{RL.Count()}");

    int maxSearch = 1000000;//arbitrary stop while debugging, was large enough in sol, kept in
    List<ulong> simplePeriods = new();
    for (int i = 0; i < startingPositions.Count(); i++)
    {
        string currPosition = startingPositions.ElementAt(i).Key; //set position on starting position
        for (int hh = 0; hh < maxSearch; hh++)//loop until all endHits have a cycle
        {
            if (Regex.IsMatch(currPosition, ends))
            {

                int totalStepsTaken = hh;
                int positionOnTape = hh % RL.Count();


                EndHit eh = new EndHit(currPosition, totalStepsTaken, positionOnTape, null);
                //Console.WriteLine($"{i},{positionOnTape},{currPosition},{hh}");
                
                foreach (EndHit prevHit in startingPositions.ElementAt(i).Value)
                {
                    if (prevHit.PositionOnTape == eh.PositionOnTape && prevHit.EndNodeId == prevHit.EndNodeId)
                    {
                        eh.StepsCountSinceLastHit = eh.AbsoluteStepsCount - prevHit.AbsoluteStepsCount;
                        if (eh.StepsCountSinceLastHit == prevHit.AbsoluteStepsCount)
                        {
                            //Nice scenario where period to get back is same as first hit
                            //The puzzle had only these... rest of code abandoned
                            //Would have to detect offsets from first hit if entering the loop took more than 1 step (first hit always known by:
                            //.StepsCountSinceLastHit being null
                            //and loop hits in case there are multiple ..Z end points for any starting pos
                            //but the puzzle said same ammount of start and end nodes
                            //could still have 2 values going into same loop with 2 Zs but it wasn't the case
                            //instead we just declare the single period for this startingPosition found and move on -- breaking out and ending search
                            ulong period = Convert.ToUInt64(eh.StepsCountSinceLastHit);
                            simplePeriods.Add(period);

                            hh = maxSearch;
                            break;
                        }
                    }
                }
                startingPositions.ElementAt(i).Value.Add(eh);
                //Console.WriteLine($"race {i} hits {currPositions[i]} ");
            }
            if (RL[Convert.ToInt32(hh % RL.Count())] == 'L')
                currPosition = nodes[currPosition].L;
            else
                currPosition = nodes[currPosition].R;
        }
    }

    ulong steps = 0;

    steps = simplePeriods.Aggregate(LCM);

    Console.WriteLine(steps);



}

ulong GCD(ulong a, ulong b)
{
    return b == 0 ? a : GCD(b, a % b);
}

ulong LCM(ulong a, ulong b)
{
    return a * b / GCD(a, b);
}

readonly struct  Node
{
    public string L { get; }
    public string R { get; }
    public Node(string l, string r) => (L, R) = (l, r);
}

struct EndHit
{
    public string EndNodeId { get; }
    //total steps to calculate offset from initial entry into loop
    public int AbsoluteStepsCount { get; }
    //index on tape
    public int PositionOnTape { get; }
    //Last hit of the same endNote at the same position on tape
    public int? StepsCountSinceLastHit { get; set; }

    public EndHit(string endNodeId, int absoluteStepsCount, int positionOnTape, int? stepsCountSinceLastHit)
        => (EndNodeId, AbsoluteStepsCount, PositionOnTape, StepsCountSinceLastHit)
        =  (endNodeId, absoluteStepsCount, positionOnTape, stepsCountSinceLastHit);
}

/* leftover from old bforce part1
bool done = false;
ulong steps = 0;
while (!done)
{
    if (RL[Convert.ToInt32(steps % Convert.ToUInt64(RL.Count()))] == 'L')
    {
        for (int q = 0; q < startingPositions.Count(); q++)
        {
            startingPositions[q] = nodes[startingPositions[q]].L;
        }
    }
    else
    {
        for (int q = 0; q < startingPositions.Count(); q++)
        {
            startingPositions[q] = nodes[startingPositions[q]].R;
        }
    }
    done = true;
    foreach (var po in startingPositions)
    {
        if (po[2] != 'Z')
        {
            done = false;
            break;
        }
    }
    steps++;
}*/