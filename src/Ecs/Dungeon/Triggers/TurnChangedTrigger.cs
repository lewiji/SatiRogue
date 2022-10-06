using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Triggers;

public class TurnChangedTrigger {
   public readonly TurnType Turn;

   public TurnChangedTrigger(TurnType turn) {
      Turn = turn;
   }
}