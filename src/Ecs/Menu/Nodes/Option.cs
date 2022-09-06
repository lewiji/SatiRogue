using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Menu.Nodes;

[Tool]
public partial class Option : Control {
   public enum OptionType { ProjectSetting, EnvironmentSetting }

   [OnReadyGet("%CheckBox")] public CheckBox CheckBox = null!;
   string _label = "";

   [Export] public string OptionLabel {
      get => _label;
      set {
         _label = value;

         if (IsInsideTree()) {
            CheckBox.Text = _label;
         }
      }
   }

   [Export] public OptionType OptionLocation { get; set; }

   [Export] public string OptionKey { get; set; } = "";

   [OnReady] void SetLabelState() {
      CheckBox.Text = _label;
   }

   [OnReady] void DisableAndroidOptions() {
      if (OS.GetName() != "Android") return;

      switch (OptionKey) {
         case "ssao_enabled":
         case "dof_blur_far_enabled,dof_blur_near_enabled":
         case "ss_reflections_enabled":
            CheckBox.Pressed = false;
            CheckBox.Disabled = true;
            break;
      }
   }

   [OnReady] void ConnectCheckbox() {
      CheckBox.Connect("toggled", this, nameof(OnCheckboxToggled));
   }

   void OnCheckboxToggled(bool pressed) {
      Owner.EmitSignal(nameof(Options.OptionChanged), (int) OptionLocation, new Dictionary {{OptionKey, pressed}});
   }
}