using Godot;

namespace SatiRogue.scenes.Debug;

public class DebugSpatialText : Spatial {
   private Label? _label;

   public override void _Ready() {
      _label = GetNode<Label>("Viewport/Control/CenterContainer/Label");
   }

   public void SetText(string text) {
      if (_label != null) _label.Text = text;
   }
}