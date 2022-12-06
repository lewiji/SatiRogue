using Godot;

namespace SatiRogue.Ecs.Dungeon.Nodes.Resources;

public partial class ItemResource : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public bool IsCellBlocker { get; set; }
    [Export] public bool IsInventory { get; set; }
    [Export] public bool IsEquippable { get; set; }
    [Export] public bool IsConsumable { get; set; }
    [Export] public Mesh? Mesh { get; set; }
    [Export] public SpriteFrames? SpriteFrames { get; set; }
    [Export] public Material? Material { get; set; }
    [Export] public CSharpScript? NodeScript { get; set; }
}