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
      GetNode("/root/Main").QueueFree();
      await ToSignal(GetTree(), "idle_frame");
      var mainScene = GD.Load<PackedScene>("res://Main.tscn").Instance();
      mainScene.Name = "Main";
      EntityRegistry.Clear();
      EntityResourceLocator.SceneNodePaths.Clear();
      GetNode("/root").AddChild(mainScene);
   }
}