using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Systems;

public class PlayerIndicatorSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      foreach (var (player, input) in this.QueryBuilder<Player, InputDirectionComponent>().Has<Aiming>().Build()) {
         if (input.Direction != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.Direction;
         } else if (input.LastDirection != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.LastDirection;
         }
      }
   }
}