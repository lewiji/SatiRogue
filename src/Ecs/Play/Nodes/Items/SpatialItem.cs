using SatiRogue.Ecs.Play.Components;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Nodes.Items;

public class SpatialItem : Item {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      entityBuilder.Add(new GridPositionComponent()).Add<Collectable>();
   }
}