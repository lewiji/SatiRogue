using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class InputSystem : ISystem {
   public World World { get; set; } = null!;

   public static bool HandlingInput = true;
   public static bool Paused = false;

   public void Run() {
      if (Paused)
         return;

      foreach (var input in this.QueryBuilder<InputDirectionComponent>().Has<Controllable>().Has<Alive>().Build()) {
         var aim = Input.IsActionPressed("aim");
         var diagonalLock = Input.IsActionPressed("diagonal_lock");
         var shoot = Input.IsActionJustPressed("shoot");
         var direction = new Vector2(0, 0); //, "move_right", "move_up", "move_down"

         if (Input.IsActionPressed("move_left")) {
            direction.x = -1;
         } else if (Input.IsActionPressed("move_right")) {
            direction.x = 1;
         }

         if (Input.IsActionPressed("move_up")) {
            direction.y = -1;
         } else if (Input.IsActionPressed("move_down")) {
            direction.y = 1;
         }

         input.Direction = direction.Round();

         if (diagonalLock) {
            HandleDiagonalLockedInput(input);
         } else {
            HandleUnlockedInput(aim, input, shoot);
         }
      }
   }

   void HandleUnlockedInput(bool aim, InputDirectionComponent input, bool shoot) {
      if (Paused)
         return;

      foreach (var (entity, player) in this.QueryBuilder<Entity, Player>().Has<DiagonalLock>().Build()) {
         this.On(entity).Remove<DiagonalLock>();
         player.DiagonalLockIndicator.Visible = false;
      }

      if (!aim && HandlingInput && input.Direction != Vector2.Zero) {
         /* It's sending a message to the `PlayerInputSystem` to tell it to run. */
         this.Send(new PlayerHasMadeInputTrigger());
         HandlingInput = false;
      } else if (shoot) {
         this.Send(new PlayerHasMadeInputTrigger());
         this.Send(new PlayerHasShotTrigger());
         HandlingInput = false;
      }

      if (aim) {
         foreach (var (entity, player) in this.QueryBuilder<Entity, Player>().Not<Aiming>().Build()) {
            this.On(entity).Add<Aiming>();
            player.DirectionIndicator.Visible = true;
         }
      } else {
         RemoveAim();
      }
   }

   void HandleDiagonalLockedInput(InputDirectionComponent input) {
      RemoveAim();

      foreach (var (entity, player) in this.QueryBuilder<Entity, Player>().Not<DiagonalLock>().Build()) {
         this.On(entity).Add<DiagonalLock>();
         player.DiagonalLockIndicator.Visible = true;
      }

      if (HandlingInput && InputIsDiagonal(input.Direction)) {
         this.Send(new PlayerHasMadeInputTrigger());
         HandlingInput = false;
      }
   }

   bool InputIsDiagonal(Vector2 inputDirection) {
      if (inputDirection == Vector2.Zero)
         return false;
      return inputDirection.y != 0 && inputDirection.x != 0;
   }

   void RemoveAim() {
      foreach (var (entity, player) in this.QueryBuilder<Entity, Player>().Has<Aiming>().Build()) {
         this.On(entity).Remove<Aiming>();
         player.DirectionIndicator.Visible = false;
      }
   }
}