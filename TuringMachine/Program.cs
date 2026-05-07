// ReSharper disable AccessToModifiedClosure

var input1 = new Input
     { Tm = "010010001010011000101010010110001001001010011000100010001010", Accepting = "11", Rejecting = "0110" };
var input2 = new Input
     { Tm = "010010100100110101000101001100010010100100110001010010100", Accepting = "100", Rejecting = "01101" };

Console.WriteLine("Input (1) or (2)?:");
var which = Console.ReadLine() == "1";
var input = which ? input1 : input2;
var tm = input.Tm;

Console.WriteLine("Accepting (A) or Rejecting (R)?");
var acc = Console.ReadLine()?.ToLower() == "a";
var data = acc ? input.Accepting : input.Rejecting;

var machine = ParseMachine(tm);

Console.WriteLine("Step-mode (s) or Run-mode (r)?:");
var stepMode = Console.ReadLine()?.ToLower() == "s";
var (state, accepted) = Execute(machine, data, stepMode);
PrintState(state);
var str = accepted ? "accepted" : "rejected";
Console.WriteLine($"Turing Machine completed with state: [{str}] in [{state.StepCount}] steps");

(State, bool) Execute(Transition[] transitions, string payload, bool mode)
{
     var stepCounter = 0;

     var currentState = 1;
     var head = 5000;
     var tapeLength = 10_000;
     int[] band = new int[tapeLength];
     Array.Fill(band, 3); // 3 = blank
     foreach (var c in payload)
     {
          if (c == '0') band[head++] = 1;
          if (c == '1') band[head++] = 2;
     }

     head = 5000;
     var stats = new State{CurrentState =  currentState, StepCount = stepCounter, Head = head};
     while (currentState != 2)
     {
          var transition = transitions.FirstOrNull(x => x.StartSymbol == band[head] && x.StartState == currentState);
          if (transition == null)
          {
               return (stats, false);
          }

          var relevantTransition = transition.Value;

          currentState = relevantTransition.TargetState;
          band[head] = relevantTransition.WriteSymbol;
          head += relevantTransition.Direction == 1 ? -1 : +1;

          var viewSize = 15;
          var viewTape = new int[viewSize];
          var rad = viewSize / 2;
          for (var i = 0; i < viewSize; i++)
          {
               var tapeIndex = head - rad + i;
               if (tapeIndex >= 0 && tapeIndex < tapeLength)
               {
                    viewTape[i] = band[tapeIndex];
               }
               else
               {
                    viewTape[i] = -1;
               }
          }

          stepCounter++;
          
          stats = new State
          {
               CurrentState = currentState,
               ViewTape = viewTape,
               Head = head,
               StepCount = stepCounter
          };

          if (mode)
          {
               PrintState(stats);
               Console.WriteLine("");
               Console.ReadKey();
          }
     }

     return (stats, true);
}

void PrintState(State stats)
{
     if (stats.ViewTape == null) return;
     Console.Clear();
     Console.WriteLine("Status: ");
     Console.WriteLine("\tcurrentState: " + stats.CurrentState);
     Console.Write("\ttape: ");
     for (var i = 0; i < stats.ViewTape.Length; i++)
     {
          string val = stats.ViewTape[i] switch
          {
               1 => "0",
               2 => "1",
               3 => "_",
               _ => ""
          };
          if (i == stats.ViewTape.Length / 2)
          {
               Console.ForegroundColor = ConsoleColor.Red;
               Console.Write($"[{val}]");
               Console.ResetColor();
          }
          else
          {
               Console.Write(val);
          }
     }

     Console.WriteLine();
     Console.WriteLine("\thead: " + stats.Head);
     Console.WriteLine("\tstepCount: " + stats.StepCount);
}

Transition[] ParseMachine(string tmAsString)
{
     int readHead = 0;
     List<Transition> transitions = [];

     if (readHead < tmAsString.Length && tmAsString[readHead] == '1')
     {
          readHead++;
     }

     while (readHead < tmAsString.Length)
     {
          transitions.Add(new Transition
          {
               StartState = ReadZeros(),
               StartSymbol = ReadZeros(),
               TargetState = ReadZeros(),
               WriteSymbol = ReadZeros(),
               Direction = ReadZeros(),
          });

          if (TryReadOnes(2))
               break;
     }

     return transitions.ToArray();

     bool TryReadOnes(int count)
     {
          int c = 0;

          while (readHead < tmAsString.Length && c < count)
          {
               if (tmAsString[readHead] != '1')
                    break;

               c++;
               readHead++;
          }

          return count == c;
     }

     int ReadZeros()
     {
          int c = 0;

          while (true)
          {
               if (readHead >= tmAsString.Length)
                    break;

               if (tmAsString[readHead] == '1')
               {
                    readHead++;
                    break;
               }

               c++;
               readHead++;
          }

          if (c == 0)
               throw new Exception("Parse Exception");

          return c;
     }
}

struct Transition
{
     public int StartState;
     public int StartSymbol;

     public int TargetState;
     public int WriteSymbol;
     public int Direction; // 1 = left, 2 = right
}

struct State
{
     public int CurrentState;
     public int[]? ViewTape;
     public int Head;
     public int StepCount;
}

struct Input
{
     public string Tm;
     public string Accepting;
     public string Rejecting;
}

public static class Extensions
{
     public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
     {
          foreach (var item in source)
          {
               if (predicate(item)) return item;
          }

          return null;
     }
}