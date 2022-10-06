using Godot;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Systems;

public class ResetInputDirectionSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      foreach (var input in this.Query<InputDirectionComponent>()) {
         input.Direction = Vector2.Zero;
      }
   }
}