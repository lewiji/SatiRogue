using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class HealthUI : Control {
   [OnReadyGet("Scale/TextureProgress")] private TextureProgress _textureProgress = null!;
   [OnReadyGet("Label")] private RichTextLabel _label = null!;
   [OnReadyGet("Timer")] private Timer _timer = null!;

   [OnReady] private void SetupTimer() {
      _timer.WaitTime = 0.382f;
      _timer.OneShot = true;
   }
   
   private float _percent;
   [Export] public float Percent {
      get => _percent;
      set {
         _percent = Mathf.Clamp(value, 0, 1);
         _textureProgress.Value = _percent * _textureProgress.MaxValue;
         
         _label.BbcodeText = $"[center][shake rate=50 level=7]{_textureProgress.Value}[/shake] / {_textureProgress.MaxValue}[/center]";

         if (!_timer.IsStopped()) {
            _timer.Disconnect("timeout", this, nameof(ResetLabelShake));
            _timer.Stop();
         }
         
         _timer.Connect("timeout", this, nameof(ResetLabelShake));
         _timer.Start();
      }
   }

   private void ResetLabelShake() {
      _label.BbcodeText = $"[center]{_textureProgress.Value} / {_textureProgress.MaxValue}[/center]";
      _timer.Disconnect("timeout", this, nameof(ResetLabelShake));
   }
}