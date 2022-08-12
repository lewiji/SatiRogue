using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Nodes.Hud;
namespace SatiRogue.Ecs.Play.Systems;

public class InventorySystem : GDSystem {
   public override void Run() {
      var invUi = GetElement<Inventory>();
      var itemSlots = invUi.GetItemSlots();

      foreach (var itemSlot in itemSlots) {
         itemSlot.Connect(nameof(ItemSlot.OnPressed), this, nameof(OnItemSlotPressed));
      }
   }

   private void OnItemSlotPressed(int index) {
      var invUi = GetElement<Inventory>();
      var itemSlot = invUi.GetItemSlot(index);
      Logger.Info($"Inventory item {itemSlot?.ItemName} clicked at index {index}.");
   }
}