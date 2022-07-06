using Godot;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen;

namespace SatiRogue.scenes.Hud; 

public partial class ConfirmStairsDialog : Control, IDependent {
   private static ConfirmationDialog? _dialogNode;
   private static TextureRect? _background;
   [Dependency] private MapGenerator _mapGenerator => this.DependOn<MapGenerator>();

   [OnReady]
   private void SetInstance() {
      _dialogNode = GetNode<ConfirmationDialog>("CenterContainer/ConfirmationDialog");
      _background = GetNode<TextureRect>("TextureRect");
      _background.Visible = false;
      _dialogNode.Connect("confirmed", this, nameof(OnConfirm));
      _dialogNode.GetCancel().Connect("pressed", this, nameof(OnCancelled));
      this.Depend();
   }

   public void Loaded() {
      Logger.Debug("ConfirmStairsDialog.cs: Received MapGenerator dependency");
   }

   public static void ConfirmStairs() {
      _dialogNode?.PopupCentered();
      InputHandlerComponent.InputEnabled = false;
      if (_background != null) _background.Visible = true;
   }

   private void OnConfirm() {
      _mapGenerator.NextFloor();
   }
   
   private void OnCancelled() {
      _dialogNode?.Hide();
   }
}