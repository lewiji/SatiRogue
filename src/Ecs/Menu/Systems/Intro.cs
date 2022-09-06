using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Intro.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Menu.Systems;

public class Intro : GdSystem {
   [Signal] public delegate void IntroFinished();

   readonly PackedScene _introScene = GD.Load<PackedScene>("res://src/Ecs/Intro/Nodes/Intro.tscn");
   Control? _intro;

   public override void Run() {
      _intro = _introScene.Instance<IntroScene>();
      _intro.Connect("ready", this, nameof(OnIntroReady));
      GetElement<MenuState>().AddChild(_intro);
   }

   void OnIntroReady() {
      Logger.Info("Playing intro");
      var player = _intro?.GetNode<AnimationPlayer>("AnimationPlayer");
      player?.Play("intro");
      player?.Connect("animation_finished", this, nameof(OnIntroFinished));
   }

   // ReSharper disable once UnusedParameter.Local
   void OnIntroFinished(string _) {
      EmitSignal(nameof(IntroFinished));
      _intro?.QueueFree();
   }
}