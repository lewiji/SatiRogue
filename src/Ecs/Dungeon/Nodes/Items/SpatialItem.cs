using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class SpatialItem : Item {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      entityBuilder.Add(new GridPositionComponent()).Add<Collectable>();
   }
}