using RelEcs;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public class Lamp : Prop {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      //entityBuilder.Add(this as Lamp);
   }
}