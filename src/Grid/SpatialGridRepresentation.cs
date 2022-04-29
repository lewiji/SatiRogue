using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.scenes;

namespace SatiRogue.Grid;

public partial class SpatialGridRepresentation : Spatial {
    private const int ChunkWidth = 24;
    private readonly Array _cellTypes = Enum.GetValues(typeof(CellType));
    [OnReadyGet("../", Export = true)] private ThreeDee? _threeDee;
    [OnReadyGet("../RoomManager", Export = true)] private RoomManager? _roomManager;

    [OnReady] private void DeferredCallToGridGeneratorSignal() {
        GD.Print("Connecting to map changed signal on next frame...");
        CallDeferred(nameof(ConnectToGridGenerator));
    }

    private void ConnectToGridGenerator() {
        GD.Print("Connecting to map changed signal.");
        var gridGenerator = _threeDee!.GridGenerator;
        gridGenerator?.Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
    }

    private void OnMapDataChanged() {
        GD.Print("3d: Map data changed");
        
        const int chunkSize = ChunkWidth * ChunkWidth;
        
        var cells = GridGenerator._mapData.Cells.ToList();
        var chunkCells = cells.Take(chunkSize).ToList();
        var chunkId = 0;
        
        GD.Print($"Chunk width: {ChunkWidth}");
        GD.Print($"{cells.Count} map cells total.");
        GD.Print($"Chunking: Taking {chunkCells.Count} cells");
        
        while (chunkCells is {Count: > 0}) {
            // Remove these cells from the enumeration
            cells?.RemoveRange(0, Mathf.Min(cells.Count, chunkSize));

            GD.Print($"{cells?.Count} cells remaining in map data");
            GD.Print($"Building chunk {chunkId}.");
            
            var chunkRoom = new Room();
            
            // TODO workout portal dimensions
            chunkRoom.AddChild(new Portal{  Points = new [] {
                new Vector2(6f, 0.5f),
                new Vector2(6f, 2.1f),
                new Vector2(-6f, 2.1f),
                new Vector2(-6f, 0.5f),
            }});

            // Create MultiMesh for each cell type in this chunk data
            foreach (CellType cellType in _cellTypes) {
                var cellsOfThisTypeInChunk = chunkCells.Where(cell => cell.CellType == cellType).ToArray();
                var meshForCellType = GetMeshResourceForCellType(cellType);
                if (meshForCellType == null) continue;

                var mmInst = new MultiMeshInstance {
                    Multimesh = new MultiMesh {
                        Mesh = meshForCellType,
                        TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
                        InstanceCount = cellsOfThisTypeInChunk.Length
                    },
                    CastShadow = GeometryInstance.ShadowCastingSetting.On
                };
                chunkRoom.AddChild(mmInst);

                for (var i = 0; i < cellsOfThisTypeInChunk.Length; i++) {
                    mmInst.Multimesh.SetInstanceTransform(i,
                        new Transform(Basis.Identity, cellsOfThisTypeInChunk[i].Position.ToVector3()));
                }
            }
            
            AddChild(chunkRoom);
            
            chunkCells = cells?.Take(chunkSize).ToList();
            if (!(chunkCells?.Count > 0)) continue;
            GD.Print($"Took {chunkCells?.Count} cells.");
            chunkId += 1;
        }
        GD.Print("Chunking finished.");
    }

    private Mesh? GetMeshResourceForCell(Cell cell) {
        return GetMeshResourceForCellType(cell.CellType);
    }

    private Mesh? GetMeshResourceForCellType(CellType? cellType) {
        return cellType switch {
            CellType.Floor => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8235.mesh"),
            CellType.Stairs => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface6972.mesh"),
            CellType.Wall => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7509.mesh"),
            CellType.DoorClosed => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7081.mesh"),
            CellType.DoorOpen => GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8475.mesh"),
            _ => null
        };
    }
}