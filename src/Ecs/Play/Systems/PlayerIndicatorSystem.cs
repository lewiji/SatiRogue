using Godot;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Systems;

public class PlayerIndicatorSystem : GdSystem {
   public override void Run() {
      foreach (var (player, input) in QueryBuilder<Nodes.Actors.Player, InputDirectionComponent>().Has<Aiming>().Build()) {
         if (input.Direction != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.Direction;
         } else if (input.LastDirection != Vector2.Zero) {
            player.DirectionIndicator.Direction = input.LastDirection;
         }
      }
   }
}