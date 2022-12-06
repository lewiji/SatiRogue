namespace SatiRogue.Ecs.Dungeon.Components;

public enum TurnType {
   PlayerTurn,
   EnemyTurn,
   Processing,
   Idle
}

public partial class Turn {
   public TurnType CurrentTurn { get; set; } = TurnType.Idle;
}