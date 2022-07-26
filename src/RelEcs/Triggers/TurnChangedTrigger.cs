namespace SatiRogue.RelEcs.Triggers; 

public class TurnChangedTrigger {
   public TurnType Turn;
   public TurnChangedTrigger(TurnType turn) {
      Turn = turn;
   }
}