using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Systems;

public class InputSystem : GDSystem {
   public static bool InputHandled = true;

   public override void Run() {
      var query = QueryBuilder<InputDirectionComponent>().Has<Controllable>().Build();

      foreach (var input in query) {
         var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
         input.Direction = direction;

         if (InputHandled && input.Direction != Vector2.Zero) {
            /* It's sending a message to the `PlayerInputSystem` to tell it to run. */
            Send(new PlayerHasMadeInputTrigger());
            InputHandled = false;
         }
      }
   }
}