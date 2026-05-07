namespace TuringMachine;

public static class Aufg2
{
     public static void Run()
     {
          TmTransition[] transitions =
          [
               // Phase 0: Convert input to source symbols
               // Replace every '1' with 'X' so we have a clean source template.
               // When we hit blank, write '#' as separator between source and output area.
               // e.g. "111" → "XXX#"
               new(0, '1', 0, 'X', Direction.R),
               new(0, 'u', 2, '#', Direction.L),

               // Phase 2: Rewind to left end
               // Just scan left over everything until we hit blank.
               new(2, 'X', 2, 'X', Direction.L),
               new(2, 'Z', 2, 'Z', Direction.L),
               new(2, '#', 2, '#', Direction.L),
               new(2, 'u', 3, 'u', Direction.R),

               // Phase 3: Pick next unspent source symbol
               // Skip past Z's (already spent rounds) to find the next X.
               // Convert it to Z — this "spends" one round, so we do exactly n rounds total.
               // If we hit '#' with no X left, all n rounds are done → halt with n² ones written.
               new(3, 'Z', 3, 'Z', Direction.R),
               new(3, 'X', 4, 'Z', Direction.L),
               new(3, '#', 8, '#', Direction.R), // halt

               // Phase 4: Rewind again before starting the copy loop
               // After marking X→Z in phase 3 we moved left one step, so rewind fully.
               // Must skip all symbol types since tape now has X, Z, A, B, #, 1 on it.
               new(4, 'Z', 4, 'Z', Direction.L),
               new(4, 'X', 4, 'X', Direction.L),
               new(4, 'A', 4, 'A', Direction.L),
               new(4, 'B', 4, 'B', Direction.L),
               new(4, '#', 4, '#', Direction.L),
               new(4, '1', 4, '1', Direction.L),
               new(4, 'u', 5, 'u', Direction.R),

               // Phase 5: Copy loop — scan source, write one '1' per symbol
               // This is the core of n²: for each round (triggered by phase 3),
               // we scan ALL n source symbols (both X and Z) and write one output '1' per symbol.
               // So each round writes exactly n ones, and we do n rounds → n×n = n² ones total.
               //
               // To avoid counting the same symbol twice in one round, we mark it:
               //   X → A  (was unspent, counted this round)
               //   Z → B  (was spent, counted this round)
               // When we hit '#', all n symbols counted → go restore (phase 7).
               new(5, 'A', 5, 'A', Direction.R), // skip already counted
               new(5, 'B', 5, 'B', Direction.R), // skip already counted
               new(5, 'X', 6, 'A', Direction.R), // count X, mark as A, go write
               new(5, 'Z', 6, 'B', Direction.R), // count Z, mark as B, go write
               new(5, '#', 7, '#', Direction.L), // all counted → restore

               // Phase 6: Travel to output end and write one '1'
               // Scan right over everything until blank, write '1', then rewind (phase 4)
               // to come back and count the next source symbol.
               new(6, 'A', 6, 'A', Direction.R),
               new(6, 'B', 6, 'B', Direction.R),
               new(6, 'X', 6, 'X', Direction.R),
               new(6, 'Z', 6, 'Z', Direction.R),
               new(6, '#', 6, '#', Direction.R),
               new(6, '1', 6, '1', Direction.R),
               new(6, 'u', 4, '1', Direction.L), // write '1', rewind for next symbol

               // Phase 7: Restore markers after a full copy round
               // A → X (still an unspent source symbol for future rounds)
               // B → Z (still a spent source symbol for future rounds)
               // Then rewind and go pick the next round (phase 2→3).
               new(7, 'A', 7, 'X', Direction.L),
               new(7, 'B', 7, 'Z', Direction.L),
               new(7, 'X', 7, 'X', Direction.L),
               new(7, 'Z', 7, 'Z', Direction.L),
               new(7, 'u', 2, 'u', Direction.R),
          ];

          var goedel = GoedelGenerator.GenerateGoedelNumber(transitions, 0, 8);

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
          Machine.PrintState(state);
          Console.WriteLine($"Result: {(accepted ? "accepted" : "rejected")} in {state.StepCount} steps");
     }
}