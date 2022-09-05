using Godot;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class ResetInputDirectionSystem : GdSystem {
   public override void Run() {
      foreach (var input in Query<InputDirectionComponent>()) {
         input.Direction = Vector2.Zero;
      }
   }
}