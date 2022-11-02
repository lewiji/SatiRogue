using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class ProjectileSystem : ISystem {
   public World World { get; set; } = null!;
   PhysicsDeltaTime? _delta;

   public void Run() {
      _delta ??= World.GetElement<PhysicsDeltaTime>();
      var query = World.Query<Entity, Arrow>().Has<Firing>().Build();

      foreach (var (entity, arrow) in query) {
         if (arrow.Translation.IsEqualApprox(arrow.Destination)) {
            this.DespawnAndFree(entity);
         } else {
            arrow.Translation = arrow.Translation.MoveToward(arrow.Destination, _delta.Value * 20f);
         }
      }
   }
}