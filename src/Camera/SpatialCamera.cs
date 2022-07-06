using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Camera;

public partial class SpatialCamera : Godot.Camera {
   private static SpatialCamera? _instance;
   private OpenSimplexNoise _noise = new OpenSimplexNoise();
   private int _noiseY = 0;
   [Export] private float ShakeDecay { get; set; } = 2f;
   [Export] private Vector2 ShakeMaxOffset { get; set; } = new Vector2(0.309f, 0.23175f);
   [Export] private float ShakeMaxRoll { get; set; } = 0.1f;
   private float _currentShakeIntensity = 0f;
   private float _shakePowerExponent = 3f;

   [OnReady]
   private void InitNoise() {
      _noise.Seed = (int)GD.Randi();
      _noise.Period = 4;
      _noise.Octaves = 2;
   }

   [OnReady]
   private void DisablePhysicsInterpolation() {
      PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
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

   private void DoShake(float intensity) {
      _currentShakeIntensity = Mathf.Min(_currentShakeIntensity + intensity, 1.0f);
   }

   public override void _Process(float delta) {
      if (_currentShakeIntensity > 0f) {
         _currentShakeIntensity = Mathf.Max(_currentShakeIntensity - ShakeDecay * delta, 0);
         ProcessShake();
      }
   }

   private void ProcessShake() {
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