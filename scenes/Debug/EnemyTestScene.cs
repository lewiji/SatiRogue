using Godot;
using SatiRogue.Entities;
namespace SatiRogue.scenes.Debug;

[Tool]
public class EnemyTestScene : Spatial {
   private AnimatedSprite3D? _enemySprite;
   private EntityTypes _entityType = EntityTypes.Harpy;

   [Export]
   public EntityTypes EntityType {
      get => _entityType;
      set {
         _entityType = value;
         LoadNewEnemyResources();
      }
   }

   public override void _Ready() {
      _enemySprite = GetNode<AnimatedSprite3D>("Enemy/AnimatedSprite3D");
   }

   private void LoadNewEnemyResources() {
      var entityResources = EntityResourceLocator.ResourceBundles[_entityType].ResourcePaths;

      if (_enemySprite != null) {
         _enemySprite.Frames = GD.Load<SpriteFrames>(entityResources["SpriteFrames"]);
         _enemySprite.MaterialOverride = GD.Load<Material>(entityResources["Material"]);
      }
   }
}