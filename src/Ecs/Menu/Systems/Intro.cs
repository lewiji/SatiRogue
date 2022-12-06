using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Intro.Nodes;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Menu.Systems;

public partial class Intro : RefCounted, ISystem {
   

   [Signal]
   public delegate void IntroFinishedEventHandler();

   readonly PackedScene _introScene = GD.Load<PackedScene>("res://src/Ecs/Intro/Nodes/Intro.tscn");
   Control? _intro;
   World? _world;
   public void Run(World world)
   {
      _world ??= world;
      _intro = _introScene.Instantiate<IntroScene>();
      _intro.Connect("ready",new Callable(this,nameof(OnIntroReady)));
      _intro.Connect(nameof(IntroScene.DebugSkipToNewGame),new Callable(this,nameof(OnSkipToNewGame)));
      world.GetElement<MenuState>().AddChild(_intro);
   }

   void OnIntroReady() {
      Logger.Info("Playing intro");
      var player = _intro?.GetNode<AnimationPlayer>("AnimationPlayer");
      player?.Play("intro");
      player?.Connect("animation_finished",new Callable(this,nameof(OnIntroFinished)));
   }

   void OnSkipToNewGame() {
      OnIntroFinished("");
      _world!.GetElement<InitMenu>().OnNewGameRequested();
   }

   // ReSharper disable once UnusedParameter.Local
   void OnIntroFinished(string _) {
      EmitSignal(nameof(IntroFinished));
      _intro?.QueueFree();
   }
}