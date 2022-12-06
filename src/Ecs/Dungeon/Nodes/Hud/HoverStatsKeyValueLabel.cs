using Godot;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class HoverStatsKeyValueLabel : HFlowContainer {
	 Label? _keyLabel;
   RichTextLabel? _valueLabel;

   string _keyName = "";
   string _value = "";

   public override void _Ready()
   {
	   _keyLabel = GetNode<Label>("KeyLabel");
	   _valueLabel = GetNode<RichTextLabel>("ValueLabel");
   }

   public async void SetKeyValue(string name, string val) {
      _keyName = name;
      _value = val;

      if (_keyLabel == null || _valueLabel == null) {
         await ToSignal(GetTree(), "idle_frame");
      }

      if (_keyLabel != null)
         _keyLabel.Text = _keyName;

      if (_valueLabel != null)
         _valueLabel.Text = _value;
   }
}