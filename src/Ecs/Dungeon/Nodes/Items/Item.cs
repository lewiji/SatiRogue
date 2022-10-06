using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Nodes.Items;

public class Item : GameObject {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Item);
   }
}