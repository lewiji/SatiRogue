using Godot;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;

namespace SatiRogue.Ecs.Play.Systems; 

public class InputSystem : GDSystem {
   public override void Run() {
         var query = QueryBuilder<PlayerInputDirectionComponent>().Has<Controllable>().Build();
         foreach (var input in query) {
            var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            input.Direction = direction;
            if (input.Direction != Vector2.Zero) {
               /* It's sending a message to the `PlayerInputSystem` to tell it to run. */
               Send(new PlayerHasMadeInputTrigger());
            }
         }
   }
}