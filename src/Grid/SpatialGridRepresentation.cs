using System;
using System.Collections.Generic;
using Godot;
using GodotOnReady.Attributes;
using RoguelikeMono.scenes.ThreeDee;

namespace RoguelikeMono.Grid;

public partial class SpatialGridRepresentation : Spatial
{
    [OnReadyGet("../", Export = true)] private ThreeDee? ThreeDee;

    private Dictionary<CellType, MultiMeshInstance> _multiMeshInstances = new Dictionary<CellType, MultiMeshInstance>();
    private Dictionary<CellType, int> _cellTypeCounts = new Dictionary<CellType, int>();
    
    [OnReady]
    private void CreateMultiMeshInstances()
    {
        GD.Print($"Creating multimeshes");
        var cellTypes = Enum.GetValues(typeof(CellType));
        foreach (CellType cellType in cellTypes)
        {
            GD.Print($"Creating multimesh for {cellType}");
            
            var meshForCellType = GetMeshResourceForCellType(cellType);
            if (meshForCellType == null) continue;
            
            var mmInst = new MultiMeshInstance();
            mmInst.Multimesh = new MultiMesh();
            mmInst.Multimesh.Mesh = meshForCellType;
            mmInst.Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
            mmInst.CastShadow = GeometryInstance.ShadowCastingSetting.On;
            AddChild(mmInst);
            _multiMeshInstances.Add(cellType, mmInst);
            _cellTypeCounts.Add(cellType, 0);
            mmInst.Multimesh.InstanceCount = 65536;
        }
    }

    [OnReady]
    private void DeferredCallToGridGeneratorSignal()
    {
        GD.Print("Connecting to map changed signal on next frame...");
        CallDeferred(nameof(ConnectToGridGenerator));
    }
    
    private void ConnectToGridGenerator() {
        GD.Print("Connecting to map changed signal.");
        var gridGenerator = ThreeDee.GridGenerator;
        gridGenerator?.Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
    }
    
    private void OnMapDataChanged()
    {
        GD.Print("3d: Mapdata changed");
        var cells = GridGenerator._mapData.Cells;
        foreach (var cell in cells)
        {
            AddMesh(new Vector3(cell.Position.x, 0, cell.Position.z), cell.CellType);
        }
    }

    private void AddMesh(Vector3 position, CellType? cellType)
    {
        if (!_multiMeshInstances.ContainsKey(cellType.Value)) return;
        
        _multiMeshInstances[cellType.Value].Multimesh.SetInstanceTransform(_cellTypeCounts[cellType.Value], new Transform(Basis.Identity, position));
        _cellTypeCounts[cellType.Value] += 1;
    }

    private Mesh GetMeshResourceForCell(Cell cell)
    {
        return GetMeshResourceForCellType(cell.CellType);
    }
    
    private Mesh? GetMeshResourceForCellType(CellType? cellType)
    {
        return cellType switch
        {
            CellType.Floor => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8235.mesh"),
            CellType.Stairs => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface6972.mesh"),
            CellType.Wall => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7509.mesh"),
            CellType.DoorClosed => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7081.mesh"),
            CellType.DoorOpen => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8475.mesh"),
            _ => null
        };
    }
}