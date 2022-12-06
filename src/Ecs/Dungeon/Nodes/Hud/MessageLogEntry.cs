using Godot;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class MessageLogEntry : MarginContainer {
   RichTextLabel? _logText;
   Timer? _timeoutTimer;
   Tween? _fadeTween;

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

   public override void _Ready()
   {
	   _logText = GetNode<RichTextLabel>("%LogText");
	   _timeoutTimer = GetNode<Timer>("Timeout");
	   _fadeTween = GetNode <Tween>("FadeTween");
	   ConnectTimer();
   }

   void SetText(string text) {
      if (_logText != null)
         _logText.Text = $"* {text}";
   }

   void ConnectTimer() {
      if (_timeoutTimer != null) {
         _timeoutTimer.Connect("timeout",new Callable(this,nameof(OnTimeout)));
         _timeoutTimer.Start(TimeoutToFade);
      }
      _fadeTween?.Connect("tween_all_completed",new Callable(this,"queue_free"));
      if (_logText != null) _logText.BbcodeEnabled = true;
   }

   void OnTimeout() {
      _fadeTween?.TweenProperty(this, "modulate",  Color.Color8(255, 255, 255, 0), FadeTime);
   }
}