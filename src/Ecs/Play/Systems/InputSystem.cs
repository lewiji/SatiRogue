using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Systems;

public class InputSystem : GDSystem {
   public static bool InputHandled = true;

   public override void Run() {
      var query = QueryBuilder<InputDirectionComponent>().Has<Controllable>().Has<Alive>().Build();

      foreach (var input in query) {
         var aim = Input.IsActionPressed("aim");
         var shoot = Input.IsActionJustPressed("shoot");
         var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
         input.Direction = direction;

         if (!aim && InputHandled && input.Direction != Vector2.Zero) {
            /* It's sending a message to the `PlayerInputSystem` to tell it to run. */
            Send(new PlayerHasMadeInputTrigger());
            InputHandled = false;
         } else if (shoot) {
            Send(new PlayerHasMadeInputTrigger());
            Send(new PlayerHasShotTrigger());
            InputHandled = false;
         }

         if (aim) {
            foreach (var (entity, player) in QueryBuilder<Entity, Nodes.Actors.Player>().Not<Aiming>().Build()) {
               On(entity).Add<Aiming>();
               player.DirectionIndicator.Visible = true;
            }
         } else {
            foreach (var (entity, player) in QueryBuilder<Entity, Nodes.Actors.Player>().Has<Aiming>().Build()) {
               GD.Print("Removing aiming");
               On(entity).Remove<Aiming>();
               player.DirectionIndicator.Visible = false;
            }
         }
      }
   }
}