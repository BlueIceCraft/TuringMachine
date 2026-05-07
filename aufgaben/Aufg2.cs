namespace TuringMachine;

public class Aufg2
{
     public static void Run()
     {
          TmTransition[] transitions = [];

          var goedel = GoedelGenerator.GenerateGoedelNumber(transitions, 0, 1);
          
          Console.WriteLine("Goedelnr: " + goedel);
          var machine = new Machine(goedel);

          Console.WriteLine("Step-mode (s) or Run-mode (r)?");
          var stepMode = Console.ReadLine()?.ToLower() == "s";

          Console.WriteLine("Enter number: ");
          int number;
          while (!int.TryParse(Console.ReadLine(), out number))
          {
               Console.WriteLine("Invalid input. Please enter a valid number: ");
          }

          var input = string.Join("", Enumerable.Repeat('1', number));
          Console.WriteLine("number: " + input);

          Console.WriteLine("Press any key to continue...");
          Console.ReadKey();

          var (state, accepted) = machine.Execute(input, stepMode);
          machine.PrintState(state);
          Console.WriteLine($"Result: {(accepted ? "accepted" : "rejected")} in {state.StepCount} steps");
     }
}