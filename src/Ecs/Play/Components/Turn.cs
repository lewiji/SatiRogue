namespace SatiRogue.Ecs.Play.Components; 

public enum TurnType {
   PlayerTurn,
   EnemyTurn,
   Processing
}

public class Turn {
   private TurnType _currentTurn = TurnType.PlayerTurn;

   public TurnType CurrentTurn {
      get => _currentTurn;
      set => _currentTurn = value;
   }
}