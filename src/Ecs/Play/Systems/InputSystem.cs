using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Triggers;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class InputSystem : GdSystem {
   public static bool HandlingInput = true;
   public static bool Paused = false;

   public override void Run() {
      if (Paused) return;

      foreach (var input in QueryBuilder<InputDirectionComponent>().Has<Controllable>().Has<Alive>().Build()) {
         var aim = Input.IsActionPressed("aim");
         var diagonalLock = Input.IsActionPressed("diagonal_lock");
         var shoot = Input.IsActionJustPressed("shoot");
         var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
         input.Direction = direction.Round();

         if (diagonalLock) {
            HandleDiagonalLockedInput(input);
         } else {
            HandleUnlockedInput(aim, input, shoot);
         }
      }
   }

   void HandleUnlockedInput(bool aim, InputDirectionComponent input, bool shoot) {
      if (Paused) return;

      foreach (var (entity, player) in QueryBuilder<Entity, Player>().Has<DiagonalLock>().Build()) {
         On(entity).Remove<DiagonalLock>();
         player.DiagonalLockIndicator.Visible = false;
      }

      if (!aim && HandlingInput && input.Direction != Vector2.Zero) {
         /* It's sending a message to the `PlayerInputSystem` to tell it to run. */
         Send(new PlayerHasMadeInputTrigger());
         HandlingInput = false;
      } else if (shoot) {
         Send(new PlayerHasMadeInputTrigger());
         Send(new PlayerHasShotTrigger());
         HandlingInput = false;
      }

      if (aim) {
         foreach (var (entity, player) in QueryBuilder<Entity, Player>().Not<Aiming>().Build()) {
            On(entity).Add<Aiming>();
            player.DirectionIndicator.Visible = true;
         }
      } else {
         RemoveAim();
      }
   }

   void HandleDiagonalLockedInput(InputDirectionComponent input) {
      RemoveAim();

      foreach (var (entity, player) in QueryBuilder<Entity, Player>().Not<DiagonalLock>().Build()) {
         On(entity).Add<DiagonalLock>();
         player.DiagonalLockIndicator.Visible = true;
      }

      if (HandlingInput && InputIsDiagonal(input.Direction)) {
         Send(new PlayerHasMadeInputTrigger());
         HandlingInput = false;
      }
   }

   bool InputIsDiagonal(Vector2 inputDirection) {
      if (inputDirection == Vector2.Zero) return false;
      return inputDirection.y != 0 && inputDirection.x != 0;
   }

   void RemoveAim() {
      foreach (var (entity, player) in QueryBuilder<Entity, Player>().Has<Aiming>().Build()) {
         On(entity).Remove<Aiming>();
         player.DirectionIndicator.Visible = false;
      }
   }
}