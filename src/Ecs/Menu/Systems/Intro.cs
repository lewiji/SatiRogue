using Godot;
using Godot.Collections;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Intro.Nodes;

namespace SatiRogue.Ecs.Menu.Systems; 

public class Intro : GdSystem {
   [Signal] public delegate void IntroFinished();
   
   private readonly PackedScene _introScene = GD.Load<PackedScene>("res://src/Ecs/Intro/Nodes/Intro.tscn");
   private Control? _intro;
   public override void Run() {
      _intro = _introScene.Instance<IntroScene>();
      _intro.Connect("ready", this, nameof(OnIntroReady));
      GetElement<MenuState>().AddChild(_intro);
   }

   void OnIntroReady() {
      Logger.Info("Playing intro");
      var player =  _intro.GetNode<AnimationPlayer>("AnimationPlayer");
      player.Play("intro");
      player.Connect("animation_finished", this, nameof(OnIntroFinished));
   }

   async void OnIntroFinished(string _) {
      await GetElement<Fade>().QuickFade();
      _intro.QueueFree();
      EmitSignal(nameof(IntroFinished));
   }
}