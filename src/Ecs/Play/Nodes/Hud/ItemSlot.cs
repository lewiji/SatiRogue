using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

[Tool]
public partial class ItemSlot : CenterContainer {
   [Signal] public delegate void OnPressed(int itemIndex);
   [OnReadyGet("TextureRect/CenterContainer")] private Control _clickTarget = null!;

   private string _itemName = "";
   private Texture? _itemTexture;
   [OnReadyGet] public Label? Label;
   [OnReadyGet] public TextureRect? TextureRect;
   [Export] public Texture? ItemTexture {
      get => _itemTexture;
      set {
         _itemTexture = value;
         if (TextureRect != null) TextureRect.Texture = _itemTexture;
      }
   }
   [Export] public string ItemName {
      get => _itemName;
      set {
         _itemName = value;
         if (Label != null) Label.Text = _itemName;
      }
   }
   [Export] public string Description { get; set; } = "";
   [Export] public int Quantity { get; set; }

   [OnReady] private void SetInitial() {
      if (TextureRect != null) TextureRect.Texture = _itemTexture;
      if (Label != null) Label.Text = _itemName;
      _clickTarget.Connect("gui_input", this, nameof(OnGuiInput));
   }

   private void OnGuiInput(InputEvent @event) {
      if (@event is InputEventMouseButton {Pressed: false} mouseJustPressed) {
         EmitSignal(nameof(OnPressed), GetIndex());
      }
   }
}