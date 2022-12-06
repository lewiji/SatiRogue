using Godot;
using Godot.Collections;
namespace SatiRogue.Ecs.Menu.Nodes;

public partial class Options : CanvasLayer {
   [Signal] public delegate void OptionChangedEventHandler(Option.OptionType optionLocation, Dictionary keyValue);
   Button _closeButton = default!;

   public override void _Ready()
   {
	   _closeButton = GetNode<Button>("%CloseButton");
	   ConnectCloseButton();
   }

   void ConnectCloseButton() {
      _closeButton.Pressed += OnClosePressed;
   }

   public override void _EnterTree() {
      Visible = false;
   }

   void OnClosePressed() {
      Hide();
   }
}