using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Loading.Nodes;

namespace SatiRogue.Ecs.Menu.Nodes; 

public partial class Options : Control {
   [Signal] public delegate void OptionChanged(Option.OptionType optionLocation, string key, bool value);
   [OnReady] private void DebugOptionChanged() {
      Connect(nameof(OptionChanged), this, nameof(OnOptionChanged));
   }

   private void OnOptionChanged(Option.OptionType optionLocation, string key, bool value) {
      GD.Print(optionLocation);
      GD.Print(key);
      GD.Print(value);
   }
}