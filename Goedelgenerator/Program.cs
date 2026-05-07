using System.Text;

//Aufgabe 1
// Transition[] transitions = [
//     new(0, '1', 1, '1', Dir.R),
//     new(1, '0', 0, '0', Dir.R),
//     new(0, 'u', 2, 'X', Dir.R),
//     new(1, 'u', 2, 'S', Dir.R),
// ];

//Aufgabe 2
// Transition[] transitions = [
//     new(0, '0', 0, '0', Dir.R),
//     new(0, '1', 0, '1', Dir.R),
//     new(0, 'u', 1, 'u', Dir.L),
//     new(1, '1', 1, '0', Dir.L),
//     new(1, '0', 2, '1', Dir.L),
//     new(1, 'u', 2, '1', Dir.L),
// ];

//Aufgabe 5
Transition[] transitions = [
    new(0, '0', 0, '0', Dir.R),
    new(0, '1', 0, '0', Dir.R),
    new(0, 'u', 0, '0', Dir.R),
];

var goedelNumber = GenerateGoedelNumber(transitions, 0, 1);
Console.WriteLine(goedelNumber);

string GenerateGoedelNumber(Transition[] input, int startState, int endState)
{
    var symbolMapping = new Dictionary<char, int>();
    var stateMapping = new Dictionary<int, int>();

    var sb = new StringBuilder();

    bool isFirst = true;
    foreach (var transition in input)
    {
        if (!isFirst)
            sb.Append("11");

        sb.Append(new string('0', GetState(transition.StartState)));
        sb.Append('1');

        sb.Append(new string('0', GetSymbol(transition.StartSymbol)));
        sb.Append('1');

        sb.Append(new string('0', GetState(transition.TargetState)));
        sb.Append('1');

        sb.Append(new string('0', GetSymbol(transition.WriteSymbol)));
        sb.Append('1');

        sb.Append(new string('0', transition.Direction == Dir.L ? 1 : 2));

        isFirst = false;
    }

    return sb.ToString();

    int GetState(int state)
    {
        if (stateMapping.TryGetValue(state, out var o))
            return o;

        if (state == startState)
            o = 1;
        else if (state == endState)
            o = 2;
        else
            o = stateMapping.Count + 3;

        stateMapping.Add(state, o);
        return o;
    }

    int GetSymbol(char symbol)
    {
        return symbol switch
        {
            '0' => 1,
            '1' => 2,
            'u' => 3,
            var c => ((Func<int>)(() =>
            {
                if (symbolMapping.TryGetValue(c, out var x))
                    return x;

                var newNum = 4 + symbolMapping.Count;
                symbolMapping.Add(c, newNum);
                return newNum;
            }))(),
        };
    }
}

record struct Transition(int StartState, char StartSymbol, int TargetState, char WriteSymbol, Dir Direction);

enum Dir
{
    L,
    R
}
