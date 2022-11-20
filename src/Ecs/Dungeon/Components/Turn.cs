namespace SatiRogue.Ecs.Dungeon.Components;

public enum TurnType {
   PlayerTurn,
   EnemyTurn,
   Processing,
   Idle
}

public class Turn {
   public TurnType CurrentTurn { get; set; } = TurnType.Idle;
}