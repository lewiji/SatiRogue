using SatiRogue.Commands;
using SatiRogue.Commands.Actions;
using SatiRogue.Entities;
using SatiRogue.Grid;
using SatiRogue.MathUtils;

namespace SatiRogue.Components; 

public class PlayerMovementComponent : MovementComponent {
   public PlayerMovementComponent(Vector3i? initialPosition = null) : base(initialPosition) { }
   
   private PlayerEntity? _parent;

   public override GameObject? Parent {
      get => _parent;
      set => _parent = value as PlayerEntity;
   }
   
   private MovementDirection? _direction;
   public override void HandleTurn() {
      if (_parent == null) return;
       Systems.TurnHandler.SetPlayerCommand(
          new ActionMove(this, _direction.GetValueOrDefault())
       );
   }

   public void SetDestination(MovementDirection movementDirection) {
      _direction = movementDirection;
      HandleTurn();
   }
}