using System.Collections.Generic;
using SatiRogue.Commands.Actions;
using SatiRogue.Entities;
using SatiRogue.MathUtils;

namespace SatiRogue.Components;

public class PlayerMovementComponent : MovementComponent {
   private MovementDirection? _direction;

   private PlayerEntity? _parent;
   public PlayerMovementComponent(Vector3i? initialPosition = null) : base(initialPosition) { }

   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new();

   public override GameObject? Parent {
      get => _parent;
      set => _parent = value as PlayerEntity;
   }

   public void SetDestination(MovementDirection movementDirection) {
      _direction = movementDirection;
      Systems.TurnHandler.SetPlayerCommand(
         new ActionMove(this, _direction.GetValueOrDefault())
      );
   }
}