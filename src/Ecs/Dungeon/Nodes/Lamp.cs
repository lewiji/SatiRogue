using RelEcs;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class Lamp : Prop {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      BlocksCell = false;
      
   }
}