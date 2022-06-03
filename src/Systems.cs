using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.Turn;

namespace SatiRogue;

public partial class Systems : Node {
   public static TurnHandler TurnHandler = new();

   [OnReady]
   private void AddSystemsToScene() {
      AddChild(TurnHandler);
   }

   public async void Restart() {
      EntityResourceLocator.SceneNodePaths.Clear();
      GetNode("/root/Main").QueueFree();
      var mainScene = GD.Load<PackedScene>("res://Main.tscn").Instance();
      await ToSignal(GetTree(), "idle_frame");
      GetNode("/root").AddChild(mainScene);
      mainScene.Name = "Main";
   }
}