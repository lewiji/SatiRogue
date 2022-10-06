using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class MessageLogEntry : MarginContainer {
   [OnReadyGet("%LogText")]
   RichTextLabel? _logText = null;

   [OnReadyGet("Timeout")]
   Timer? _timeoutTimer = null;

   [OnReadyGet("FadeTween")]
   Tween? _fadeTween = null;

   const float TimeoutToFade = 2f;
   const float FadeTime = 3f;

   string _messageText = "";
   public string Text {
      get => _messageText;
      set {
         _messageText = value;

         if (_logText != null)
            SetText(_messageText);
         else
            CallDeferred(nameof(SetText), _messageText);
      }
   }

   void SetText(string text) {
      if (_logText != null)
         _logText.Text = $"* {text}";
   }

   [OnReady]
   void ConnectTimer() {
      if (_timeoutTimer != null) {
         _timeoutTimer.Connect("timeout", this, nameof(OnTimeout));
         _timeoutTimer.Start(TimeoutToFade);
      }
      _fadeTween?.Connect("tween_all_completed", this, "queue_free");
   }

   void OnTimeout() {
      _fadeTween?.InterpolateProperty(this, "modulate", Modulate, Color.Color8(255, 255, 255, 0), FadeTime);
      _fadeTween?.Start();
   }
}