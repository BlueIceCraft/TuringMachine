namespace TuringMachine;

public record struct TmTransition(int StartState, char StartSymbol, int TargetState, char WriteSymbol, Direction Direction);

public enum Direction
{
    L,
    R 
}
