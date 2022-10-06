using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class HoverStatsKeyValueLabel : HFlowContainer {
   [OnReadyGet("KeyLabel")]
   Label? _keyLabel;

   [OnReadyGet("ValueLabel")]
   RichTextLabel? _valueLabel;

   string _keyName = "";
   string _value = "";

   public async void SetKeyValue(string name, string val) {
      _keyName = name;
      _value = val;

      if (_keyLabel == null || _valueLabel == null) {
         await ToSignal(GetTree(), "idle_frame");
      }

      if (_keyLabel != null)
         _keyLabel.Text = _keyName;

      if (_valueLabel != null)
         _valueLabel.BbcodeText = _value;
   }
}