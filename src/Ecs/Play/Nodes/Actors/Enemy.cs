using Godot;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public class Enemy : Character {
   public SpriteFrames? Frames;
   public Material? Material;
   public Stats Stats = new(1, 10, 1, 1, 0);

   public override void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this)
         .Add(this as Character)
         .Add(Stats)
         .Add(new HealthComponent(Stats.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new BehaviourTree())
         .Add<Walkable>()
         .Add<Alive>();
   }

   public override void _Ready() {
      base._Ready();

      if (AnimatedSprite3D == null) return;
      AnimatedSprite3D.Frames = Frames;
      AnimatedSprite3D.MaterialOverride = Material;

      if (WallPeekSprite == null) return;
      WallPeekSprite.Frames = Frames;
      var peekMat = Material?.Duplicate() as SpatialMaterial;
      WallPeekSprite.MaterialOverride = peekMat;

      if (peekMat != null) {
         peekMat.FlagsNoDepthTest = true;
         peekMat.DistanceFadeMode = SpatialMaterial.DistanceFadeModeEnum.PixelDither;
         peekMat.DistanceFadeMinDistance = 0f;
         peekMat.DistanceFadeMaxDistance = 16f;
      }
   }
}