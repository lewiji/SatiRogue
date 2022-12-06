using System.Collections.Generic;
using Godot;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class MessageLog : Control {
   static readonly PackedScene MessageEntryScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/MessageLogEntry.tscn");

   Control _messageContainer = default!;
   ScrollContainer _scrollContainer = default!;
   Timer _timeoutToFadeOutTimer = default!;
   Tween _fadeTween = default!;
   List<string> _messageHistory = new();

   public override void _Ready()
   {
	   _messageContainer = GetNode<Control>("%MessageContainer");
	   _scrollContainer  = GetNode<ScrollContainer>("%ScrollContainer");
		   _timeoutToFadeOutTimer = GetNode<Timer>("FadeTimeout");
	   _fadeTween = GetNode<Tween>("FadeTween");
	   SetInitialAlpha();
	   ConnectTimer();
   }

   void SetInitialAlpha() {
      Modulate = Color.Color8(255, 255, 255, 0);
   }

   void ConnectTimer() {
      _timeoutToFadeOutTimer?.Connect("timeout",new Callable(this,nameof(FadeOut)));
   }

   public async void AddMessage(string text) {
	   _messageHistory.Add(text);
      var log = MessageEntryScene.Instantiate<MessageLogEntry>();
      log.Text = text;
      _messageContainer.AddChild(log);

      if (Modulate.a8 < 255)
         FadeIn();
      _timeoutToFadeOutTimer?.Start();

      await ToSignal(GetTree(), "idle_frame");
      _scrollContainer?.EnsureControlVisible(log);
   }

   async void FadeIn() {
      if (_fadeTween.IsRunning())
         _fadeTween.Kill();
      var timeScale = 1f - Modulate.a8 / 255f;
      _fadeTween.TweenProperty(this, "modulate",  Color.Color8(255, 255, 255, 255), 0.31f * timeScale);
   }

   async void FadeOut() {
      if (_fadeTween.IsRunning())
         _fadeTween.Kill();
      var timeScale = Modulate.a8 / 255f;
      _fadeTween.TweenProperty(this, "modulate",  Color.Color8(255, 255, 255, 0), 0.618f * timeScale);
   }
}