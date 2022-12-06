using Godot;
namespace SatiRogue.scenes;

public partial class LoadEnvironment : WorldEnvironment {
	public override void _Ready()
	{
		Environment = GD.Load<Environment>("res://scenes/res/EnvironmentBase.tres");
	}
}