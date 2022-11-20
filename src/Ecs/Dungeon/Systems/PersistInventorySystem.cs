using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class PersistInventorySystem : ISystem {
   

   public void Run(World world) {
      var query = world.Query<Entity, Item>().Has<JustPickedUp>().Has<InInventory>().Build();

      foreach (var (entity, item) in query) {
         world.GetElement<PersistentPlayerData>().AddItem(item);
         world.RemoveComponent<JustPickedUp>(entity);
         item.GetParent().RemoveChild(item);
      }
   }
}