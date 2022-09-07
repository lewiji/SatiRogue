using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Menu.Nodes;

public partial class Options : CanvasLayer {
   [Signal] public delegate void OptionChanged(Option.OptionType optionLocation, Dictionary keyValue);
   [OnReadyGet("%CloseButton")] Button _closeButton = null!;

   [OnReady] void ConnectCloseButton() {
      _closeButton.Connect("pressed", this, nameof(OnClosePressed));
   }

   public override void _EnterTree() {
      Visible = false;
   }

   void OnClosePressed() {
      Hide();
   }
}