namespace TuringMachine;

public class Aufg1
{
     public static void Run()
     {
          Console.WriteLine("Input: (1) or (2)?");
          var input = Console.ReadLine() == "1";
          var tm = input
               ? "010010001010011000101010010110001001001010011000100010001010"
               : "010010100100110101000101001100010010100100110001010010100";
          var accepting = input ? "11" : "100";
          var rejecting = input ? "0110" : "01101";

          Console.WriteLine("Accepting (A) or Rejecting (R)?");
          var isAccepting = Console.ReadLine()?.ToLower() == "a";
          var testData = isAccepting ? accepting : rejecting;

          Console.WriteLine("Step-mode (s) or Run-mode (r)?");
          var stepMode = Console.ReadLine()?.ToLower() == "s";

          var machine = new Machine(tm);
          var (state, accepted) = machine.Execute(testData, stepMode);
          machine.PrintState(state);
          var result = accepted ? "accepted" : "rejected";
          Console.WriteLine($"Turing Machine completed with state: [{result}] in [{state.StepCount}] steps");
     }
}