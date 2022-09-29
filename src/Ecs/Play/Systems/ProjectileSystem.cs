using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Items;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Systems;

public class ProjectileSystem : ISystem {
   public World World { get; set; } = null!;
   PhysicsDeltaTime? _delta;

   public void Run() {
      _delta ??= World.GetElement<PhysicsDeltaTime>();
      var query = this.QueryBuilder<Entity, Arrow>().Has<Firing>().Build();

      foreach (var (entity, arrow) in query) {
         if (arrow.Translation.IsEqualApprox(arrow.Destination)) {
            this.DespawnAndFree(entity);
         } else {
            arrow.Translation = arrow.Translation.MoveToward(arrow.Destination, _delta.Value * 20f);
         }
      }
   }
}