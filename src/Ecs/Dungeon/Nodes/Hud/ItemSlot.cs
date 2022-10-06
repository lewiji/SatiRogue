using System.Diagnostics.CodeAnalysis;
using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
public partial class ItemSlot : CenterContainer {
   [Signal] public delegate void OnPressed(int itemIndex);

   AtlasTexture _normalFrameTex = GD.Load<AtlasTexture>("res://src/Ecs/Dungeon/Nodes/Hud/ItemFrameNormal.tres");
   AtlasTexture _focusFrameTex = GD.Load<AtlasTexture>("res://src/Ecs/Dungeon/Nodes/Hud/ItemFrameSelected.tres");

   [OnReadyGet("%ClickTarget")] Control _clickTarget = null!;
   [OnReadyGet("%Equipped")] Control _equippedIndicator = null!;
   [OnReadyGet("%ItemTexture")] TextureRect _itemTextureRect = null!;
   [OnReadyGet("%Frame")] TextureRect _frameTextureRect = null!;
   [OnReadyGet("%LabelName")] Label _labelName = null!;
   [OnReadyGet("%PopupMenu")] PopupMenu _popupMenu = null!;

   public bool IsEmpty {
      get => _itemTexture == null;
   }

   bool _isSelected;
   public bool IsSelected {
      get => !IsEmpty && _isSelected;
      set {
         if (IsEmpty) {
            _isSelected = false;
            _frameTextureRect.Texture = _normalFrameTex;
         } else {
            _isSelected = value;
            _frameTextureRect.Texture = _isSelected ? _focusFrameTex : _normalFrameTex;
         }
         SetEmptyModulation();
      }
   }

   Texture? _itemTexture;
   [Export] public Texture? ItemTexture {
      get => _itemTexture;
      set {
         _itemTexture = value;
         _itemTextureRect.Texture = _itemTexture;
         SetEmptyModulation();
      }
   }

   string _itemName = "";
   [Export] public string ItemName {
      get => _itemName;
      set {
         _itemName = value;
         _labelName.Text = _itemName;
      }
   }

   bool _equipped;
   [Export] public bool Equipped {
      get => _equipped;
      set {
         _equipped = value;
         _equippedIndicator.Visible = _equipped;
      }
   }

   [Export] public string Description { get; set; } = "";
   [Export] public int Quantity { get; set; }

   [OnReady] void SetInitial() {
      _itemTextureRect.Texture = _itemTexture;
      _labelName.Text = _itemName;
      _clickTarget.Connect("gui_input", this, nameof(OnGuiInput));
      _popupMenu.Connect("about_to_show", this, nameof(OnSelected));
      _popupMenu.Connect("popup_hide", this, nameof(OnDeselected));
      SetEmptyModulation();
   }

   void SetEmptyModulation() {
      _frameTextureRect.Modulate = IsEmpty ? new Color("7f9e9e9e") : new Color("ffffff");
   }

   void OnSelected() {
      IsSelected = true;
   }

   void OnDeselected() {
      IsSelected = false;
   }

   void OnGuiInput(InputEvent @event) {
      if (@event is InputEventMouseButton {Pressed: false} inputEventMouseButton) {
         if (IsEmpty) return;

         EmitSignal(nameof(OnPressed), GetIndex());

         _popupMenu.Popup_(new Rect2(inputEventMouseButton.GlobalPosition, new Vector2(55, 74)));
      }

      @event.Dispose();
   }
}