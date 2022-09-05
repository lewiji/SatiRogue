using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Items;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class ProjectileSystem : GdSystem {
   PhysicsDeltaTime? _delta;

   public override void Run() {
      _delta ??= GetElement<PhysicsDeltaTime>();
      var query = QueryBuilder<Entity, Arrow>().Has<Firing>().Build();

      foreach (var (entity, arrow) in query) {
         if (arrow.Translation.IsEqualApprox(arrow.Destination)) {
            DespawnAndFree(entity);
         } else {
            arrow.Translation = arrow.Translation.MoveToward(arrow.Destination, _delta.Value * 20f);
         }
      }
   }
}