using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class SpatialCamera : Camera3D {
   static SpatialCamera? _instance;
   FastNoiseLite _noise = new();
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

   Node3D Target { get; set; } = null!;

   public override void _Ready()
   {
	   Target = GetNode<Node3D>("../../");
	   SetInitialOffset();
	   TeleportInitially();
	   InitNoise();
   }

   void SetInitialOffset() {
      _offset = GlobalPosition - Target.GlobalPosition;
      TopLevel = true;
      Position = Target.GlobalPosition + _offset;
   }

   async void TeleportInitially() {
      if (Smoothed) {
         Smoothed = false;
         await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
         Smoothed = true;
      }
   }

   void InitNoise()
   {
	   _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
      _noise.Seed = (int) GD.Randi();
      _noise.Frequency = 4f;
      _noise.FractalOctaves = 2;
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

   public override void _PhysicsProcess(double delta) {
      if (_currentShakeIntensity > 0f) {
         _currentShakeIntensity = Mathf.Max(_currentShakeIntensity - ShakeDecay * (float)delta, 0);
         ProcessShake();
      }


      if (!Smoothed) {
         Position = Target.GlobalPosition + _offset;
         return;
      }
      
      var targetTranslation = Target.GlobalTransform.origin + _offset;

      var distanceSquared = Position.DistanceSquaredTo(targetTranslation);

      if (distanceSquared < 0.0005f) {
         Position = Target.GlobalPosition + _offset;
         return;
      }
      Position = Position.Lerp(targetTranslation, (float)delta * SmoothSpeed);
   }

   void ProcessShake() {
      _noiseY += 1;
      var amount = Mathf.Pow(_currentShakeIntensity, _shakePowerExponent);
      Rotation = new Vector3(Rotation.x, Rotation.y, ShakeMaxRoll * amount * _noise.GetNoise2d(_noise.Seed, _noiseY));
      HOffset = ShakeMaxOffset.x * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
      VOffset = ShakeMaxOffset.y * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
   }
}