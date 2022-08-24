using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Menu.Nodes;

namespace SatiRogue.Ecs.Loading.Nodes; 

[Tool]
public partial class Option : Control {
   [OnReadyGet("%CheckBox")] private CheckBox _checkBox = null!;
   
   public enum OptionType { ProjectSetting, EnvironmentSetting }
   
   private string _label = "";
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

   [OnReady] private void SetLabelText() {
      _checkBox.Text = _label;
   }

   [OnReady] private void ConnectCheckbox() {
      _checkBox.Connect("toggled", this, nameof(OnCheckboxToggled));
   }

   private void OnCheckboxToggled(bool pressed) {
      Owner.EmitSignal(nameof(Options.OptionChanged), (int)OptionLocation, OptionKey, pressed);
   }
}