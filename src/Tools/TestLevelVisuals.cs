using Godot;
using SatiRogue.Ecs.Dungeon.Nodes.Resources;

namespace SatiRogue.Debug; 

[Tool]
public class TestLevelVisuals : Spatial
{
    private Resource? _levelSet;
    [Export] private Resource? LevelSet {
        get => _levelSet;
        set {
            _levelSet = value;
            if (_levelSet is LevelMaterialSet levelSet) {
                ChangeMaterialSet(levelSet);
            } else if (value != null) {
                GD.Print($"{_levelSet} was not a LevelMaterialSet.");
            }
        }
    }
    
    [Export] private Mesh? FloorTileMesh { get; set; }
    [Export] private Mesh? WallMesh { get; set; }
    [Export] private Mesh? StairsMesh { get; set; }
    
    public override void _Ready()
    {
        
    }

    void ChangeMaterialSet(LevelMaterialSet materialSet) {
        GD.Print("Changing material set");
        if (FloorTileMesh != null && materialSet.FloorMaterial is { } floorMaterial) {
            FloorTileMesh.SurfaceSetMaterial(0, floorMaterial);
        }
        
        if (WallMesh != null && materialSet.WallMaterial is { } wallMaterial) {
            WallMesh.SurfaceSetMaterial(0, wallMaterial);
        }
        
        if (StairsMesh != null && materialSet.StairsMaterial is { } stairsMaterial) {
            StairsMesh.SurfaceSetMaterial(0, stairsMaterial);
        } 
    }
}