using SatiRogue.Ecs.Play.Components;
namespace SatiRogue.Ecs.MapGenerator.Triggers;

public class TurnChangedTrigger {
   public readonly TurnType Turn;

   public TurnChangedTrigger(TurnType turn) {
      Turn = turn;
   }
}