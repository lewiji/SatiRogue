using Godot;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class Loot : Control {
   int _numLoots;
   RichTextLabel _richTextLabel = null!;
   public int NumLoots {
      get => _numLoots;
      set {
         _numLoots = value;
         _richTextLabel.Text = $" {_numLoots}";
      }
   }

   public override void _Ready()
   {
	   _richTextLabel = GetNode<RichTextLabel>("HBoxContainer/RichTextLabel");
	   SetInitial();
	   ConnectGuiInput();
   }

   void SetInitial() { }

   void ConnectGuiInput() {
      Connect("gui_input",new Callable(this,nameof(OnGuiInput)));
   }

   void OnGuiInput(InputEvent @event) {
      if (@event is InputEventMouseButton {Pressed: false}) {
         var invGui = GetParent().GetNode<Inventory>("Inventory");
         invGui.Toggle();
      }
      @event.Dispose();
   }
}