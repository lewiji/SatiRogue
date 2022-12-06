using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class Prop : GameObject {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Prop).Add(new GridPositionComponent());
   }
}