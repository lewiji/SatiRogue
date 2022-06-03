using System;
using Godot;
using SatiRogue.Entities;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render;

public class EntityPositionMarkerComponent : SpatialRendererComponent
{
    private Sprite3D _sprite3D;
    
    protected override void CreateRootNode()
    {
        if (GridEntity == null) return;
        if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
        var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
        if (threeDeeNode.HasNode("Markers")) {
            RootNode = threeDeeNode.GetNode<Spatial>("Markers");
        }
        else {
            RootNode = new Spatial() {Name = "Markers"};
            threeDeeNode.AddChild(RootNode);
        }
    }
    protected override void CreateVisualNodes() {
        if (GridEntity == null) throw new Exception("No parent entity found for EntityPositionMarkerComponent");

        _sprite3D = GD.Load<PackedScene>("res://scenes/ThreeDee/EntityPositionMarker.tscn").Instance<Sprite3D>();
        var pos = GridEntity.GridPosition.ToVector3();
        _sprite3D.Translation = new Vector3(pos.x, 2f, pos.z);
        _sprite3D.Name = GridEntity.Uuid;
        RootNode?.AddChild(_sprite3D);
    }

    private void MoveMarker()
    {
        var pos = GridEntity!.GridPosition.ToVector3();
        _sprite3D.Translation = new Vector3(pos.x, 2f, pos.z);   
    }
    
    public override void HandleTurn() {
        if (GridEntity == null || RootNode == null) return;
        CallDeferred(nameof(MoveMarker));
    }
}