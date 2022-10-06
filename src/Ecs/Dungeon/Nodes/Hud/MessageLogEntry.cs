using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Dungeon.Nodes.Hud; 

public partial class MessageLogEntry : MarginContainer {
   [OnReadyGet("%LogText")] private RichTextLabel? _logText = null;
   [OnReadyGet("Timeout")] private Timer? _timeoutTimer = null;
   [OnReadyGet("FadeTween")] private Tween? _fadeTween = null;
   
   private string _messageText = "";
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
      if (_logText != null) _logText.Text = $"* {text}";
   }

   [OnReady] void ConnectTimer() {
      _timeoutTimer?.Connect("timeout", this, nameof(OnTimeout));
      _fadeTween?.Connect("tween_all_completed", this, "hide");
   }

   void OnTimeout() {
      _fadeTween?.InterpolateProperty(this, "modulate", Modulate, Color.Color8(255, 255, 255, 0), 0.3f);
      _fadeTween?.Start();
   }
}