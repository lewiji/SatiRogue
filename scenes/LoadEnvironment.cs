using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.scenes; 

public partial class LoadEnvironment : WorldEnvironment
{
    [OnReady] private void OpenEnvironment() {
        Environment = GD.Load<Environment>("res://scenes/res/EnvironmentBase.tres");
    }
}