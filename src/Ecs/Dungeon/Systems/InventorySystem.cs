using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class InventorySystem : RefCounted, ISystem {
   
   Inventory? _inventoryUi;
   World? _world;
   
   public void Run(World world)
   {
      _world ??= world;
      var persistentStore = world.GetElement<PersistentPlayerData>();
      GD.Print($"===> persistent items: {persistentStore.GetItems().Count}");

      foreach (var item in persistentStore.GetItems()) {
         GD.Print(item);
         GD.Print(IsInstanceValid(item));
      }

      GD.Print($"<=== persistent items");

      var invUi = world.GetElement<Inventory>();
      _inventoryUi = invUi;
      var itemSlots = invUi.GetItemSlots();

      foreach (var itemSlot in itemSlots) {
         itemSlot.Connect(nameof(ItemSlot.OnPressed),new Callable(this,nameof(OnItemSlotPressed)));
      }

      invUi.Connect(nameof(Inventory.OpenChanged),new Callable(this,nameof(OnInventoryOpenChanged)));
   }

   void OnInventoryOpenChanged(bool isOpen) {
      if (_inventoryUi == null)
         return;

      if (isOpen) {
         var persistentStore = _world!.GetElement<PersistentPlayerData>();
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
      var invUi = _world!.GetElement<Inventory>();
      var itemSlot = invUi.GetItemSlot(index);
      Logger.Info($"Inventory item {itemSlot?.ItemName} clicked at index {index}.");
   }
}