using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Play.Nodes;

public partial class SpatialCamera : Camera {
   static SpatialCamera? _instance;
   OpenSimplexNoise _noise = new();
   int _noiseY;

   [Export]
   float ShakeDecay { get; set; } = 2f;

   [Export]
   Vector2 ShakeMaxOffset { get; set; } = new(0.309f, 0.23175f);

   [Export]
   float ShakeMaxRoll { get; set; } = 0.1f;
   float _currentShakeIntensity;
   float _shakePowerExponent = 3f;

   Vector3 _offset;

   [Export]
   bool Smoothed { get; set; } = true;

   [Export]
   float SmoothSpeed { get; set; } = 10f;

   [OnReadyGet("../../")]
   Spatial Target { get; set; } = null!;

   [OnReady]
   void DisablePhysicsInterpolation() {
      PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
      _offset = GlobalTranslation - Target.GlobalTranslation;
      SetAsToplevel(true);
      Translation = Target.GlobalTranslation + _offset;
   }

   [OnReady]
   async void TeleportInitially() {
      if (Smoothed) {
         Smoothed = false;
         await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
         Smoothed = true;
      }
   }

   [OnReady]
   void InitNoise() {
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

      var targetTranslation = Target.GetGlobalTransformInterpolated().origin + _offset;

      if (!Smoothed) {
         Translation = targetTranslation;
         return;
      }

      var distanceSquared = Translation.DistanceSquaredTo(targetTranslation);

      if (distanceSquared < 0.0025f) {
         Translation = targetTranslation;
         return;
      }

      var smoothSpeed = SmoothSpeed;

      switch (distanceSquared) {
         case >= 1f:
            smoothSpeed *= 4.854f;
            break;
         case <= 0.01f:
            smoothSpeed *= 2.617924f;
            break;
         case <= 0.03f:
            smoothSpeed *= 1.618f;
            break;
      }

      Translation = Translation.LinearInterpolate(targetTranslation, delta * smoothSpeed);
   }

   void ProcessShake() {
      _noiseY += 1;
      var amount = Mathf.Pow(_currentShakeIntensity, _shakePowerExponent);
      Rotation = new Vector3(Rotation.x, Rotation.y, ShakeMaxRoll * amount * _noise.GetNoise2d(_noise.Seed, _noiseY));
      HOffset = ShakeMaxOffset.x * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
      VOffset = ShakeMaxOffset.y * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
   }

   public override void _Input(InputEvent @event) {
      if (@event is not InputEventMouseButton {Pressed: true} inputEventMouseButton)
         return;

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