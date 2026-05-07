namespace TuringMachine;

class Machine
{
     private readonly Transition[] _transitions;
     private readonly int _tapeLength;

     public Machine(string tmInput)
     {
          _transitions = ParseMachine(tmInput);
          _tapeLength = 10_000;
     }

     public (State, bool) Execute(string testData, bool stepMode)
     {
          var stepCounter = 0;
          var currentState = 1;
          var head = 5000;
          int[] band = new int[_tapeLength];
          Array.Fill(band, 3); // 3 = blank
          foreach (var c in testData)
          {
               if (c == '0') band[head++] = 1;
               if (c == '1') band[head++] = 2;
          }

          head = 5000;
          var stats = new State { CurrentState = currentState, StepCount = stepCounter, Head = head };
          while (currentState != 2)
          {
               var transition = _transitions.FirstOrNull(x => x.StartSymbol == band[head] && x.StartState == currentState);
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
                    if (tapeIndex >= 0 && tapeIndex < _tapeLength)
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

               if (stepMode)
               {
                    PrintState(stats);
                    Console.WriteLine("");
                    Console.ReadKey();
               }
          }

          return (stats, true);
     }

     public void PrintState(State stats)
     {
          if (stats.ViewTape == null) return;
          //Console.Clear();
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

     private Transition[] ParseMachine(string tmAsString)
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