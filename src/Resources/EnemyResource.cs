using Godot;

namespace SatiRogue.Resources;

public class EnemyResource : Resource
{
    [Export] public EnemyData.Species Species { get; set; }
    [Export] public string Name { get; set; } = "";
    [Export] public SpriteFrames? SpriteFrames { get; set; }
    [Export] public EnemyData.Level Level { get; set; } = EnemyData.Level.LowlyEnemy;
}