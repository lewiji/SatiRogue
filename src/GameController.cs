using Godot;
using SatiRogue.Debug;
using SatiRogue.Entities;

namespace SatiRogue; 

public class GameController : Node {
   public static string? Path;

   public override void _EnterTree() {
      Path = GetPath();
   }

   public override void _ExitTree() {
      Path = null;
   }
   public async void Restart() {
      Logger.Warn("--- RESTART REQUESTED ---");
      EntityRegistry.Clear();
      EntityResourceLocator.SceneNodePaths.Clear();
      GetNode("/root/Main").QueueFree();
      await ToSignal(GetTree(), "idle_frame");
      await ToSignal(GetTree(), "idle_frame");
      var mainScene = GD.Load<PackedScene>("res://Main.tscn").Instance();
      mainScene.Name = "Main";
      GetNode("/root").AddChild(mainScene);
   }
}