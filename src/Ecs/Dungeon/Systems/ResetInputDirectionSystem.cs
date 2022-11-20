using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class ResetInputDirectionSystem : ISystem {
   

   public void Run(World world) {
      foreach (var input in world.Query<InputDirectionComponent>().Build()) {
         input.Direction = Vector2.Zero;
      }
   }
}