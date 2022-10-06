using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class ResetInputDirectionSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      foreach (var input in this.Query<InputDirectionComponent>()) {
         input.Direction = Vector2.Zero;
      }
   }
}