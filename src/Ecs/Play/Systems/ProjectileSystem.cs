using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Items;
namespace SatiRogue.Ecs.Play.Systems;

public class ProjectileSystem : GdSystem {
   PhysicsDeltaTime? _delta;

   public override void Run() {
      _delta ??= GetElement<PhysicsDeltaTime>();
      var query = QueryBuilder<Entity, Arrow, InputDirectionComponent, GridPositionComponent>().Has<Firing>().Build();

      foreach (var (entity, arrow, input, gridPos) in query) {
         //arrow.Translate(new Vector3(input.Direction.x, 0, input.Direction.y));

         //if (arrow.Translation.DistanceSquaredTo(gridPos.Position) >= 25f) {
         //   On(entity).Remove<Firing>();
         //}

         if (arrow.Translation.IsEqualApprox(arrow.Destination)) {
            DespawnAndFree(entity);
         } else {
            arrow.Translation = arrow.Translation.MoveToward(arrow.Destination, _delta.Value * 20f);
         }
      }
   }
}