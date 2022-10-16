namespace SatiRogue.Ecs.Dungeon.Components;

public enum TurnType {
   PlayerTurn,
   EnemyTurn,
   Processing,
   Idle
}

public class Turn {
   TurnType _currentTurn = TurnType.Idle;

   public TurnType CurrentTurn {
      get => _currentTurn;
      set => _currentTurn = value;
   }
}