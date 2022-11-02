using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class PersistInventorySystem : ISystem {
   public World World { get; set; }

   public void Run() {
      var query = World.Query<Entity, Item>().Has<JustPickedUp>().Has<InInventory>().Build();

      foreach (var (entity, item) in query) {
         World.GetElement<PersistentPlayerData>().AddItem(item);
         World.RemoveComponent<JustPickedUp>(entity);
         item.GetParent().RemoveChild(item);
      }
   }
}