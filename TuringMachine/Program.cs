// ReSharper disable AccessToModifiedClosure

var trans = "010010001010011000101010010110001001001010011000100010001010";
var data = "1";
var input = trans + "111" + data;
var (machine, startOfW) = ParseMachine(input);

var res = Execute(machine, startOfW);

Console.WriteLine($"Turing machine completed in {res} steps");

int Execute(Transition[] transitions, int startOfW)
{
     int stepCounter = 0;

     int currentState = 1;
     int head = 5000;
     int[] band = new int[10_000];
     Array.Fill(band, 3); // 3 = blank
     foreach (var c in input)
     {
          if (c == '0') band[head++] = 1;
          if (c == '1') band[head++] = 2;
     }

     head = 5000 + startOfW;

     while (currentState != 2)
     {
          var transition = transitions.FirstOrNull(x => x.StartSymbol == band[head] && x.StartState == currentState);
          if (transition == null)
          {
               Console.WriteLine("Didn't find transition");
               break;
          }

          var relevantTransition = transition.Value;

          currentState = relevantTransition.TargetState;
          band[head] = relevantTransition.WriteSymbol;
          head += relevantTransition.Direction == 1 ? -1 : +1;

          stepCounter++;
     }

     return stepCounter;
}

(Transition[], int startOfW) ParseMachine(string input)
{
     int readHead = 0;

     List<Transition> transitions = [];

     while (readHead < input.Length)
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

     return (transitions.ToArray(), readHead);

     bool TryReadOnes(int count)
     {
          int c = 0;

          while (readHead < input.Length && c < count)
          {
               if (input[readHead] != '1')
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
               if (readHead >= input.Length)
                    break;

               if (input[readHead] == '1')
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