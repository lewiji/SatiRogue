using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class PlayerIndicatorSystem : ISystem {
   

   public void Run(World world) {
      foreach (var (player, input) in world.Query<Player, InputDirectionComponent>().Has<Aiming>().Build()) {
         if (input.Direction != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.Direction;
         } else if (input.LastDirection != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.LastDirection;
         }
      }
   }
}