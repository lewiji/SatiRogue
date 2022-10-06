using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class InventorySystem : Reference, ISystem {
   public World World { get; set; } = null!;
   Inventory? _inventoryUi;

   public void Run() {
      var persistentStore = World.GetElement<PersistentPlayerData>();
      GD.Print($"===> persistent items: {persistentStore.GetItems().Count}");

      foreach (var item in persistentStore.GetItems()) {
         GD.Print(item);
         GD.Print(IsInstanceValid(item));
      }

      GD.Print($"<=== persistent items");

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
         var persistentStore = World.GetElement<PersistentPlayerData>();
         _inventoryUi.ClearSlots();

         foreach (var item in persistentStore.GetItems()) {
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