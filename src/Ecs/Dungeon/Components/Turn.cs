namespace SatiRogue.Ecs.Play.Components;

public enum TurnType {
   PlayerTurn,
   EnemyTurn,
   Processing,
   Idle
}

public class Turn {
   TurnType _currentTurn = TurnType.PlayerTurn;

   public TurnType CurrentTurn {
      get => _currentTurn;
      set => _currentTurn = value;
   }
}