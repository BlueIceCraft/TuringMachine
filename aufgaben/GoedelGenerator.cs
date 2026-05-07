using System.Text;

namespace TuringMachine;

public static class GoedelGenerator
{
     public static string GenerateGoedelNumber(TmTransition[] transitions, int startState, int endState)
     {
          var symbolMapping = new Dictionary<char, int>();
          var stateMapping = new Dictionary<int, int>();

          var sb = new StringBuilder();

          var isFirst = true;

          foreach (var transition in transitions)
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

               sb.Append(new string('0', transition.Direction == Direction.L ? 1 : 2));

               isFirst = false;
          }
          
          return sb.ToString();

          int GetState(int state)
          {
               if (state == startState) return 1;
               if (state == endState) return 2;
    
               if (stateMapping.TryGetValue(state, out var o))
                    return o;

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
                    var c => GetOrAdd(c)
               };

               int GetOrAdd(char c)
               {
                    if (symbolMapping.TryGetValue(c, out var x))
                         return x;

                    var newNum = 4 + symbolMapping.Count;
                    symbolMapping.Add(c, newNum);
                    return newNum;
               }
          }
     }
}