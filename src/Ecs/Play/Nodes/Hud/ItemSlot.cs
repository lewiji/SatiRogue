using System.Diagnostics.CodeAnalysis;
using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
[Tool]
public partial class ItemSlot : CenterContainer {
   [Signal] public delegate void OnPressed(int itemIndex);

   private AtlasTexture _normalFrameTex = GD.Load<AtlasTexture>("res://src/Ecs/Play/Nodes/Hud/ItemFrameNormal.tres");
   private AtlasTexture _focusFrameTex = GD.Load<AtlasTexture>("res://src/Ecs/Play/Nodes/Hud/ItemFrameSelected.tres");

   [OnReadyGet("%ClickTarget")] private Control _clickTarget = null!;
   [OnReadyGet("%Equipped")] private Control _equippedIndicator = null!;
   [OnReadyGet("%ItemTexture")] private TextureRect _itemTextureRect = null!;
   [OnReadyGet("%Frame")] private TextureRect _frameTextureRect = null!;
   [OnReadyGet("%LabelName")] private Label _labelName = null!;
   [OnReadyGet("%PopupMenu")] private PopupMenu _popupMenu = null!;

   public bool IsEmpty {
      get => _itemTexture == null;
   }

   private bool _isSelected = false;
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

   private Texture? _itemTexture;
   [Export] public Texture? ItemTexture {
      get => _itemTexture;
      set {
         _itemTexture = value;
         _itemTextureRect.Texture = _itemTexture;
         SetEmptyModulation();
      }
   }

   private string _itemName = "";
   [Export] public string ItemName {
      get => _itemName;
      set {
         _itemName = value;
         _labelName.Text = _itemName;
      }
   }

   private bool _equipped;
   [Export] public bool Equipped {
      get => _equipped;
      set {
         _equipped = value;
         _equippedIndicator.Visible = _equipped;
      }
   }

   [Export] public string Description { get; set; } = "";
   [Export] public int Quantity { get; set; }

   [OnReady] private void SetInitial() {
      _itemTextureRect.Texture = _itemTexture;
      _labelName.Text = _itemName;
      _clickTarget.Connect("gui_input", this, nameof(OnGuiInput));
      _popupMenu.Connect("about_to_show", this, nameof(OnSelected));
      _popupMenu.Connect("popup_hide", this, nameof(OnDeselected));
      SetEmptyModulation();
   }

   private void SetEmptyModulation() {
      _frameTextureRect.Modulate = IsEmpty ? new Color("7f9e9e9e") : new Color("ffffff");
   }

   private void OnSelected() {
      IsSelected = true;
   }

   private void OnDeselected() {
      IsSelected = false;
   }

   private void OnGuiInput(InputEvent @event) {
      if (@event is not InputEventMouseButton {Pressed: false} inputEventMouseButton) return;
      if (IsEmpty) return;

      EmitSignal(nameof(OnPressed), GetIndex());

      _popupMenu.Popup_(new Rect2(inputEventMouseButton.GlobalPosition, new Vector2(55, 74)));
   }
}