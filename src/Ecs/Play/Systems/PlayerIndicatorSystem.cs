using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class PlayerIndicatorSystem : GdSystem {
   public override void Run() {
      foreach (var (player, input) in QueryBuilder<Player, InputDirectionComponent>().Has<Aiming>().Build()) {
         if (input.Direction != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.Direction;
         } else if (input.LastDirection != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.LastDirection;
         }
      }
   }
}