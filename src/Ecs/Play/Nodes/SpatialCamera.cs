using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Play.Nodes;

public partial class SpatialCamera : Camera {
   static SpatialCamera? _instance;
   OpenSimplexNoise _noise = new();
   int _noiseY = 0;
   [Export] float ShakeDecay { get; set; } = 2f;
   [Export] Vector2 ShakeMaxOffset { get; set; } = new(0.309f, 0.23175f);
   [Export] float ShakeMaxRoll { get; set; } = 0.1f;
   float _currentShakeIntensity = 0f;
   float _shakePowerExponent = 3f;
   [OnReadyGet("../../")] Spatial Target { get; set; }

   [OnReady] void InitNoise() {
      _noise.Seed = (int) GD.Randi();
      _noise.Period = 4;
      _noise.Octaves = 2;
   }

   public static void Shake(float intensity = 1f) {
      _instance?.DoShake(intensity);
   }

   public override void _EnterTree() {
      _instance = this;
   }

   public override void _ExitTree() {
      _instance = null;
   }

   void DoShake(float intensity) {
      _currentShakeIntensity = Mathf.Min(_currentShakeIntensity + intensity, 1.0f);
   }

   public override void _Process(float delta) {
      if (_currentShakeIntensity > 0f) {
         _currentShakeIntensity = Mathf.Max(_currentShakeIntensity - ShakeDecay * delta, 0);
         ProcessShake();
      }
   }

   void ProcessShake() {
      _noiseY += 1;
      var amount = Mathf.Pow(_currentShakeIntensity, _shakePowerExponent);
      Rotation = new Vector3(Rotation.x, Rotation.y, ShakeMaxRoll * amount * _noise.GetNoise2d(_noise.Seed, _noiseY));
      HOffset = ShakeMaxOffset.x * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
      VOffset = ShakeMaxOffset.y * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
   }

   public override void _Input(InputEvent @event) {
      if (@event is not InputEventMouseButton {Pressed: true} inputEventMouseButton) return;

      switch (inputEventMouseButton.ButtonIndex) {
         case (int) ButtonList.WheelUp:
            Fov -= 1f;
            break;
         case (int) ButtonList.WheelDown:
            Fov += 1f;
            break;
      }
   }
}