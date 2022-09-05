using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Menu.Nodes;

[Tool]
public partial class Option : Control {
   public enum OptionType { ProjectSetting, EnvironmentSetting }

   [OnReadyGet("%CheckBox")] CheckBox _checkBox = null!;
   string _label = "";

   [Export] public string OptionLabel {
      get => _label;
      set {
         _label = value;

         if (IsInsideTree()) {
            _checkBox.Text = _label;
         }
      }
   }

   [Export] public OptionType OptionLocation { get; set; }

   [Export] public string OptionKey { get; set; } = "";

   [OnReady] void SetLabelState() {
      _checkBox.Text = _label;
   }

   [OnReady] void ConnectCheckbox() {
      _checkBox.Connect("toggled", this, nameof(OnCheckboxToggled));
   }

   void OnCheckboxToggled(bool pressed) {
      Owner.EmitSignal(nameof(Options.OptionChanged), (int) OptionLocation, new Dictionary {{OptionKey, pressed}});
   }
}