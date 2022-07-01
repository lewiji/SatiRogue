using System.Collections.Generic;
using System.Linq;
using SatiRogue.Commands.Actions;
using SatiRogue.Entities;
using SatiRogue.MathUtils;

namespace SatiRogue.Components;

public class PlayerMovementComponent : MovementComponent {
   private MovementDirection? _direction;

   private PlayerEntity? _playerEntity;
   public PlayerMovementComponent(Vector3i? initialPosition = null) : base(initialPosition) { }

   public override GameObject? EcOwner {
      get => _playerEntity;
      set => _playerEntity = value as PlayerEntity;
   }

   public void SetDestination(MovementDirection movementDirection) {
      _direction = movementDirection;
      var targetCellOccupants = TestMoveForOccupants(_direction.GetValueOrDefault());
      switch (targetCellOccupants?.Length) {
         case 0:
            Systems.TurnHandler.SetPlayerCommand(
               new ActionMove(_playerEntity!, _direction.GetValueOrDefault())
            );
            break;
         case > 0 when targetCellOccupants.First() is EnemyEntity enemyEntity:
            Systems.TurnHandler.SetPlayerCommand(
               new ActionAttack(_playerEntity!, enemyEntity)
            );
            break;
         case > 0 when targetCellOccupants.Last() is StairsEntity stairsEntity:
            Systems.TurnHandler.SetPlayerCommand(
               new ActionMove(_playerEntity!, _direction.GetValueOrDefault())
            );
            break;
         default:
            Systems.TurnHandler.SetPlayerCommand(
               new ActionDoNothing(_playerEntity!)
            );
            break;
      }
   }

   public override void HandleTurn()
   {
      _direction = null;
   }
}