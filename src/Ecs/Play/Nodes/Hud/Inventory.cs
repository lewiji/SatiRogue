using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class Inventory : Control {
   [Signal] public delegate void OpenChanged(bool isOpen);
   private static readonly PackedScene ItemSlotScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Item.tscn");
   [OnReadyGet("AnimationPlayer")] private AnimationPlayer? _animationPlayer;
   [OnReadyGet("CenterContainer/MarginContainer/ItemGrid/GridContainer")] private GridContainer? _gridContainer;
   private bool _isOpen;
   private int _numItemSlots;
   [Export] public int NumItemSlots {
      get => _numItemSlots;
      set {
         _numItemSlots = value;

         CallDeferred(nameof(InitialiseItemSlots));
      }
   }
   public bool IsOpen {
      get => _isOpen;
      private set {
         _isOpen = value;
         EmitSignal(nameof(OpenChanged), _isOpen);
      }
   }

   private void InitialiseItemSlots() {
      if (_gridContainer != null && _numItemSlots != _gridContainer.GetChildCount()) {
         CreateItemSlots();
      } else {
         Logger.Error("Failed to create debug slots for inventory");
      }
   }

   private void CreateItemSlots() {
      if (_gridContainer == null) {
         GD.Print("gridContainer not initialised for inventory");
         return;
      }
      var oldItemSlots = _gridContainer.GetChildren();

      foreach (Node oldItemSlot in oldItemSlots) {
         oldItemSlot.QueueFree();
      }

      for (var i = 0; i < NumItemSlots; i++) {
         var itemSlot = ItemSlotScene.Instance<ItemSlot>();
         _gridContainer.AddChild(itemSlot);
      }
   }

   public void ClearSlots() {
      var slots = GetItemSlots();

      foreach (var itemSlot in slots) {
         itemSlot.ItemTexture = null;
      }
   }

   public ItemSlot? GetFirstUnusedSlot() {
      var slots = GetItemSlots();

      foreach (var itemSlot in slots) {
         if (itemSlot.ItemTexture == null)
            return itemSlot;
      }

      return null;
   }

   public ItemSlot? GetItemSlot(int index) {
      return _gridContainer?.GetChildOrNull<ItemSlot>(index);
   }

   public ItemSlot[] GetItemSlots() {
      if (_gridContainer == null) {
         GD.Print("gridContainer not initialised for inventory");
         return new ItemSlot[] { };
      }
      var childs = _gridContainer.GetChildren();
      return childs.Cast<ItemSlot>().ToArray();
   }

   public void Open() {
      if (IsOpen) return;
      _animationPlayer?.Play("open");
      IsOpen = true;
   }

   public void Close() {
      if (!IsOpen) return;
      _animationPlayer?.Play("close");
      IsOpen = false;
   }

   public void Toggle() {
      if (IsOpen) Close();
      else Open();
   }

   public void SetOpen(bool isOpen) {
      if (isOpen == IsOpen) return;

      if (isOpen) Open();
      else Close();
   }
}