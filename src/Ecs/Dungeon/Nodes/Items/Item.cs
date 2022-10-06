using RelEcs;

namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public class Item : GameObject {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Item);
   }
}