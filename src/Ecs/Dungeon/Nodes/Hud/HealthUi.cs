using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class HealthUi : Control {
   TextureProgressBar _textureProgress = null!;
   RichTextLabel _label = null!;
   Timer _timer = null!;

   void SetupTimer() {
      _timer.WaitTime = 0.382f;
      _timer.OneShot = true;
   }

   float _percent;
   [Export] public float Percent {
      get => _percent;
      set {
         _percent = Mathf.Clamp(value, 0, 1);
         _textureProgress.Value = _percent * _textureProgress.MaxValue;

         _label.Text = $"[center][shake rate=50 level=7]{_textureProgress.Value}[/shake] / {_textureProgress.MaxValue}[/center]";

         if (!_timer.IsStopped()) {
            _timer.Disconnect("timeout",new Callable(this,nameof(ResetLabelShake)));
            _timer.Stop();
         }

         _timer.Connect("timeout",new Callable(this,nameof(ResetLabelShake)));
         _timer.Start();
      }
   }

   public override void _Ready()
   {
	   _textureProgress = GetNode<TextureProgressBar>("Scale/TextureProgressBar");
	   _label = GetNode<RichTextLabel>("Label");
	   _timer = GetNode<Timer>("Timer");
	   SetupTimer();

   }

   void ResetLabelShake() {
      _label.Text = $"[center]{_textureProgress.Value} / {_textureProgress.MaxValue}[/center]";
      _timer.Disconnect("timeout",new Callable(this,nameof(ResetLabelShake)));
   }
}