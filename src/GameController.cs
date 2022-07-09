using Godot;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Entities;

namespace SatiRogue; 

public class GameController : Node {

   public async void Restart() {
      Logger.Warn("--- RESTART REQUESTED ---");
      InputHandlerComponent.InputEnabled = false;
      EntityResourceLocator.SceneNodePaths.Clear();
      GetNode("/root/Main").QueueFree();
      await ToSignal(GetTree(), "idle_frame");
      await ToSignal(GetTree(), "idle_frame");
      var mainScene = GD.Load<PackedScene>("res://Main.tscn").Instance();
      mainScene.Name = "Main";
      GetNode("/root").AddChild(mainScene);
   }
}