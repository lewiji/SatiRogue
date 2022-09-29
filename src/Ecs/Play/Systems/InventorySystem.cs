using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Nodes.Items;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class InventorySystem : Reference, ISystem {
   public World World { get; set; } = null!;
   Inventory? _inventoryUi;

   public void Run() {
      var invUi = World.GetElement<Inventory>();
      _inventoryUi = invUi;
      var itemSlots = invUi.GetItemSlots();

      foreach (var itemSlot in itemSlots) {
         itemSlot.Connect(nameof(ItemSlot.OnPressed), this, nameof(OnItemSlotPressed));
      }

      invUi.Connect(nameof(Inventory.OpenChanged), this, nameof(OnInventoryOpenChanged));
   }

   void OnInventoryOpenChanged(bool isOpen) {
      if (_inventoryUi == null)
         return;

      if (isOpen) {
         _inventoryUi.ClearSlots();
         var query = this.QueryBuilder<Item>().Has<InInventory>().Build();

         foreach (var item in query) {
            var texture = item.GetNode<AnimatedSprite3D>("AnimatedSprite3D").Frames.GetFrame("default", 0);

            if (_inventoryUi.GetFirstUnusedSlot() is { } itemSlot) {
               itemSlot.ItemTexture = texture;
            }
         }
      }
   }

   void OnItemSlotPressed(int index) {
      var invUi = World.GetElement<Inventory>();
      var itemSlot = invUi.GetItemSlot(index);
      Logger.Info($"Inventory item {itemSlot?.ItemName} clicked at index {index}.");
   }
}