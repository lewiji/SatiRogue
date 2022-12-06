using Godot;

namespace SatiRogue.Ecs.Dungeon.Nodes.Resources; 

[Tool]
public partial class LevelMaterialSet : Resource
{
   [Export] public Material? WallMaterial { get; set; }
   [Export] public Material? FloorMaterial { get; set; }
   [Export] public Material? StairsMaterial { get; set; }
}