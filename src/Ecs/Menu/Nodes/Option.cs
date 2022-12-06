using Godot;
using Godot.Collections;
namespace SatiRogue.Ecs.Menu.Nodes;

[Tool]
public partial class Option : Control {
   public enum OptionType { ProjectSetting, EnvironmentSetting }
   public CheckBox CheckBox = default!;
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

   public override void _Ready()
   {
	   CheckBox = GetNode<CheckBox>("%CheckBox");
	   SetLabelState();
	   DisableAndroidOptions();
	   ConnectCheckbox();
   }

   void SetLabelState() {
      CheckBox.Text = _label;
   }

   void DisableAndroidOptions() {
      if (OS.GetName() != "Android") return;

      switch (OptionKey) {
         case "ssao_enabled":
         case "dof_blur_far_enabled,dof_blur_near_enabled":
         case "ss_reflections_enabled":
            CheckBox.ButtonPressed = false;
            CheckBox.Disabled = true;
            break;
      }
   }

   void ConnectCheckbox()
   {
	   CheckBox.Toggled += OnCheckboxToggled;
   }

   void OnCheckboxToggled(bool pressed) {
      Owner.EmitSignal(Options.SignalName.OptionChanged, (int) OptionLocation, new Dictionary {{OptionKey, pressed}});
   }
}