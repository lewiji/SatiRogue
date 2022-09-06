using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Nodes.Items;

public class Item : GameObject {
   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this);
   }
}