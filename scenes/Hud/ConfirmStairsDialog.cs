using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components;

namespace SatiRogue.scenes.Hud; 

public partial class ConfirmStairsDialog : Control {
   private static ConfirmationDialog? _dialogNode;
   private static TextureRect? _background;

   [OnReady]
   private void SetInstance() {
      _dialogNode = GetNode<ConfirmationDialog>("CenterContainer/ConfirmationDialog");
      _background = GetNode<TextureRect>("TextureRect");
      _background.Visible = false;
   }

   public static void ConfirmStairs() {
      _dialogNode?.PopupCentered();
      InputHandlerComponent.InputEnabled = false;
      if (_background != null) _background.Visible = true;
   }
}