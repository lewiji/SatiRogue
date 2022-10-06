using Godot;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Items;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class PersistInventorySystem : ISystem {
   public World World { get; set; }

   public void Run() {
      var query = this.QueryBuilder<Entity, Item>().Has<JustPickedUp>().Has<InInventory>().Build();

      foreach (var (entity, item) in query) {
         World.GetElement<PersistentPlayerData>().AddItem(item);
         World.RemoveComponent<JustPickedUp>(entity.Identity);
         item.GetParent().RemoveChild(item);
      }
   }
}