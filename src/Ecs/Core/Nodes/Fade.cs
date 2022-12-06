using System.Threading.Tasks;
using Godot;
namespace SatiRogue.Ecs.Core.Nodes;

public partial class Fade : CanvasLayer {
   AnimationPlayer _animationPlayer = null!;
   PointLight2D _light2D = null!;
   Timer _lightTimer = null!;
   float _dt;

   bool _animatingDown = true;

   public override void _Ready()
   {
	   _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	   _light2D = GetNode<PointLight2D>("%PointLight2D");
	   _lightTimer = GetNode<Timer>("%LightAnimationTimer");
	   ConnectLightTimer();
   }

   void ConnectLightTimer() {
      _lightTimer.Connect("timeout",new Callable(this,nameof(OnLightTimer)));
   }

   public async Task QuickFade() {
      _lightTimer.Start();
      _animationPlayer.Play("quick_fade");
      await ToSignal(_animationPlayer, "animation_finished");
   }

   public async Task FadeToBlack() {
      _lightTimer.Start();
      _animationPlayer.Play("fade_to_black");
      await ToSignal(_animationPlayer, "animation_finished");
   }

   public async Task FadeFromBlack() {
      _animationPlayer.Play("fade_from_black");
      await ToSignal(_animationPlayer, "animation_finished");
      CallDeferred(nameof(ResetLight));
   }

   void ResetLight() {
      _lightTimer.Stop();
      _light2D.Offset = new Vector2(0, 0);
      _dt = 0;
      _animationPlayer.Play("RESET");
   }

   void OnLightTimer() {
      _dt += (float)_lightTimer.WaitTime;

      if (_animatingDown) {
         _light2D.Offset += new Vector2(0, 1);

         if (_light2D.Offset.y >= 72) {
            _animatingDown = false;
         }
      } else {
         _light2D.Offset -= new Vector2(0, 1);

         if (_light2D.Offset.y <= -16) {
            _animatingDown = true;
         }
      }
   }
}