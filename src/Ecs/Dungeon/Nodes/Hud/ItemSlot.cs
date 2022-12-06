using System.Diagnostics.CodeAnalysis;
using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
public partial class ItemSlot : CenterContainer {
   [Signal] public delegate void OnPressedEventHandler(int itemIndex);

   AtlasTexture _normalFrameTex = GD.Load<AtlasTexture>("res://src/Ecs/Dungeon/Nodes/Hud/ItemFrameNormal.tres");
   AtlasTexture _focusFrameTex = GD.Load<AtlasTexture>("res://src/Ecs/Dungeon/Nodes/Hud/ItemFrameSelected.tres");

   Control _clickTarget = default!;
   Control _equippedIndicator = default!;
   TextureRect _itemTextureRect = default!;
   TextureRect _frameTextureRect = default!;
   Label _labelName = default!;
   PopupMenu _popupMenu = default!;

   public bool IsEmpty {
      get => _itemTexture == null;
   }

   public override void _Ready()
   {
	    _clickTarget = GetNode<Control>("%ClickTarget");
	    _equippedIndicator = GetNode<Control>("%Equipped");
	    _itemTextureRect = GetNode<TextureRect>("%ItemTexture");
	    _frameTextureRect = GetNode<TextureRect>("%Frame");
	    _labelName = GetNode<Label>("%LabelName");
	    _popupMenu = GetNode<PopupMenu>("%PopupMenu");
	    SetInitial();
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

   Texture2D? _itemTexture;
   [Export] public Texture2D? ItemTexture {
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

   void SetInitial() {
      _itemTextureRect.Texture = _itemTexture;
      _labelName.Text = _itemName;
      _clickTarget.Connect("gui_input",new Callable(this,nameof(OnGuiInput)));
      _popupMenu.Connect("about_to_show",new Callable(this,nameof(OnSelected)));
      _popupMenu.Connect("popup_hide",new Callable(this,nameof(OnDeselected)));
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

         _popupMenu.Popup(new Rect2i((Vector2i)inputEventMouseButton.GlobalPosition, new Vector2i(55, 74)));
      }

      @event.Dispose();
   }
}