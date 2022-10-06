using System.Collections.Generic;
using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class MessageLog : Control {
   static readonly PackedScene MessageEntryScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/MessageLogEntry.tscn");

   [OnReadyGet("%MessageContainer")]
   Control? _messageContainer = null;

   [OnReadyGet("%ScrollContainer")]
   ScrollContainer? _scrollContainer = null;

   [OnReadyGet("FadeTimeout")]
   Timer? _timeoutToFadeOutTimer = null;

   [OnReadyGet("FadeTween")]
   Tween? _fadeTween = null;
   List<string> _messageHistory = new();

   [OnReady]
   void SetInitialAlpha() {
      Modulate = Color.Color8(255, 255, 255, 0);
   }

   [OnReady]
   void ConnectTimer() {
      _timeoutToFadeOutTimer?.Connect("timeout", this, nameof(FadeOut));
   }

   public async void AddMessage(string text) {
      if (_messageContainer == null)
         return;

      _messageHistory.Add(text);
      var log = MessageEntryScene.Instance<MessageLogEntry>();
      log.Text = text;
      _messageContainer.AddChild(log);

      if (Modulate.a8 < 255)
         FadeIn();
      _timeoutToFadeOutTimer?.Start();

      await ToSignal(GetTree(), "idle_frame");
      _scrollContainer?.EnsureControlVisible(log);
   }

   async void FadeIn() {
      if (_fadeTween != null && _fadeTween.IsActive())
         _fadeTween.RemoveAll();
      var timeScale = 1f - Modulate.a8 / 255f;
      _fadeTween?.InterpolateProperty(this, "modulate", Modulate, Color.Color8(255, 255, 255, 255), 0.31f * timeScale);
      _fadeTween?.Start();
   }

   async void FadeOut() {
      if (_fadeTween != null && _fadeTween.IsActive())
         _fadeTween.RemoveAll();
      var timeScale = Modulate.a8 / 255f;
      _fadeTween?.InterpolateProperty(this, "modulate", Modulate, Color.Color8(255, 255, 255, 0), 0.618f * timeScale);
      _fadeTween?.Start();
   }
}