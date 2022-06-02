using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Entities;
using SatiRogue.MathUtils;
using SatiRogue.scenes.Hud;

namespace SatiRogue.scenes.Debug; 

[Tool]
public class EnemyTestScene : Spatial
{
    private EntityTypes _entityType = EntityTypes.Harpy;
    private AnimatedSprite3D? _enemySprite;
    private StatBar3D? _statBar3D;

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
        _statBar3D = GetNode<StatBar3D>("Enemy/StatBar3D");
    }

    private void LoadNewEnemyResources() {
        var entityResources = EntityResourceLocator.ResourceBundles[_entityType].ResourcePaths;
        if (_enemySprite != null) {
            _enemySprite.Frames = GD.Load<SpriteFrames>(entityResources["SpriteFrames"]);
            _enemySprite.MaterialOverride = GD.Load<Material>(entityResources["Material"]);
        }
        
    }
}