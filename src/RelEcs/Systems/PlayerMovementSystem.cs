using Godot;
using RelEcs;
using SatiRogue.RelEcs.Components;

namespace SatiRogue.RelEcs.Systems; 

public class PlayerMovementSystem : GodotSystem {
   public override void Run() {
      var query = QueryBuilder<InputDirectionComponent>().Has<Controllable>().Build();
      foreach (var input in query) {
         var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
         input.Direction = direction;
      }
   }
}