using SatiRogue.Ecs.Play.Components;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Nodes.Items;

public class SpatialItem : Item {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      entityBuilder.Add(this).Add(new GridPositionComponent()).Add<Collectable>();
   }
}