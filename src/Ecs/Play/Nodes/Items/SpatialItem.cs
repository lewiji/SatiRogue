using SatiRogue.Ecs.Play.Components;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Nodes.Items;

public class SpatialItem : Item {
   public override void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this).Add(this as Item).Add(new GridPositionComponent()).Add<Collectable>();
   }
}