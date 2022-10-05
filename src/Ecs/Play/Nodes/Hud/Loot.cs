using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class Loot : Control {
   int _numLoots;
   [OnReadyGet("HBoxContainer/RichTextLabel")] RichTextLabel _richTextLabel = null!;
   public int NumLoots {
      get => _numLoots;
      set {
         _numLoots = value;
         _richTextLabel.BbcodeText = $" {_numLoots}";
      }
   }

   [OnReady] void SetInitial() { }

   [OnReady] void ConnectGuiInput() {
      Connect("gui_input", this, nameof(OnGuiInput));
   }

   void OnGuiInput(InputEvent @event) {
      if (@event is InputEventMouseButton {Pressed: false}) {
         var invGui = GetParent().GetNode<Inventory>("Inventory");
         invGui.Toggle();
      }
      @event.Dispose();
   }
}