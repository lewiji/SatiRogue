using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Math;
using SatiRogue.scenes;

namespace SatiRogue.Grid;

public partial class SpatialGridRepresentation : Spatial {
    private int ChunkWidth = 15;
    private readonly Array _cellTypes = Enum.GetValues(typeof(CellType));

    private readonly SpatialMaterial _debugPortalMaterial =
        GD.Load<SpatialMaterial>("res://scenes/res/debug/DebugPortalMeshMaterial.tres");

    [OnReadyGet("../", Export = true)] private ThreeDee? _threeDee;

    [OnReadyGet("../RoomManager", Export = true)]
    private RoomManager? _roomManager;

    [OnReady]
    private void DeferredCallToGridGeneratorSignal() {
        Logger.Print("Connecting to map changed signal on next frame...");
        CallDeferred(nameof(ConnectToGridGenerator));
    }

    private void ConnectToGridGenerator() {
        Logger.Print("Connecting to map changed signal.");
        var gridGenerator = _threeDee!.GridGenerator;
        gridGenerator?.Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
    }

    private Vector3i[] GetChunkMinMaxCoords(int chunkId, int maxWidth) {
        var start = new Vector3i(
            (chunkId * ChunkWidth) % (maxWidth),
            0,
            Mathf.FloorToInt((chunkId * (float)ChunkWidth) / maxWidth) * ChunkWidth);

        var end = start + new Vector3i(ChunkWidth, 0, ChunkWidth);
        var coords = new[] { start, end };
        return coords;
    }

    private bool ChunkPositionCondition(Cell c, Vector3i[] chunkCoords) {
        return c.Position.x >= chunkCoords[0].x && c.Position.x <= chunkCoords[1].x &&
               c.Position.z >= chunkCoords[0].z && c.Position.z <= chunkCoords[1].z;
    }

    private void OnMapDataChanged() {
        Logger.Print("3d: Map data changed");

        var cells = GridGenerator._mapData.Cells.ToList();
        var mapParams = GridGenerator.GetParams();
        var maxWidth = mapParams["Width"];
        int chunkSize = ChunkWidth * ChunkWidth;
        var totalChunks = Mathf.CeilToInt(((mapParams["Width"] + ChunkWidth) * (mapParams["Height"] + ChunkWidth)) / (float)chunkSize);

        Logger.Print($"Chunk width: {ChunkWidth}");
        Logger.Print($"Max width: {maxWidth}");
        Logger.Print($"{cells.Count} map cells total.");
        Logger.Print($"Total chunks: {totalChunks}");

        for (var chunkId = 0; chunkId < totalChunks; chunkId++) {
            var chunkCoords = GetChunkMinMaxCoords(chunkId, maxWidth + ChunkWidth);
            var chunkCells = cells.Where(c => ChunkPositionCondition(c, chunkCoords)).ToList();
            Logger.Print($"Chunking: Taking {chunkCells.Count} cells");
            Logger.Print("---");
            // Remove these cells from the enumeration
            cells = cells?.Except(chunkCells).ToList();

            Logger.Print($"{cells?.Count} cells remaining in map data");
            Logger.Print($"Building chunk {chunkId}.");

            var chunkRoom = new Room {
                Name = $"Chunk{chunkId}"
            };
            AddChild(chunkRoom);
            chunkRoom.Owner = this;
            chunkRoom.Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z);


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
                mmInst.Owner = this;


                for (var i = 0; i < cellsOfThisTypeInChunk.Length; i++) {
                    mmInst.Multimesh.SetInstanceTransform(i,
                        new Transform(Basis.Identity, cellsOfThisTypeInChunk[i].Position.ToVector3() - chunkRoom.Translation));
                }
            }


            var boundSize = new Vector3(ChunkWidth, ChunkWidth, ChunkWidth);
            var boundMesh = new MeshInstance {Name = $"Bound_Chunk{chunkId}"};
            boundMesh.Translation = new Vector3(ChunkWidth / 2f, ChunkWidth / 2f, ChunkWidth / 2f);
            boundMesh.Mesh = new CubeMesh {
                Size = boundSize
            };
            boundMesh.MaterialOverride = _debugPortalMaterial;
            boundMesh.PortalMode = CullInstance.PortalModeEnum.Ignore;
            boundMesh.IncludeInBound = false;
            //boundMesh.Translation = new Vector3(-ChunkWidth / 2f, -ChunkWidth / 2f, -ChunkWidth / 2f);
            chunkRoom.AddChild(boundMesh);
            boundMesh.Owner = this;

            var portals = CreatePortals();
            foreach (var portal in portals) {
                chunkRoom.AddChild(portal);
                portal.Owner = this;
                var debugPortalMesh = CreateDebugPortalMesh(portal);
                chunkRoom.AddChild(debugPortalMesh);
                debugPortalMesh.Visible = false;
                debugPortalMesh.Owner = this;
            }

            chunkRoom.Connect("gameplay_entered", this, nameof(OnGameplayEntered), new Godot.Collections.Array {chunkRoom});
        }

        Logger.Print("Chunking finished.");

        _roomManager?.RoomsConvert();

        var packed = new PackedScene();
        packed.Pack(this);
        ResourceSaver.Save("res://testgeometry.scn", packed);
    }

    private void OnGameplayEntered(Node room) {
        GD.Print($"Entered {room.Name}");
    }

    private void OnBodyEntered(Node body, string name) {
        GD.Print($"{body.GetParent().Name} entered chunk {name}");
    }

    private IEnumerable<Portal> CreatePortals() {
        var portalSize = ChunkWidth / 2f;
        var portalSouth = new Portal {
            Name = "PortalSouth",
            Points = new[] {
                new Vector2(portalSize, -6f),
                new Vector2(portalSize, 16f),
                new Vector2(-portalSize, 16f),
                new Vector2(-portalSize, -6f)
            }
        };
        portalSouth.RotationDegrees = new Vector3(0, 180f, 0);
        portalSouth.Translation = new Vector3(portalSize + 1f, 0, ChunkWidth + 1f);

        var portalEast = new Portal {
            Name = "PortalEast",
            Points = new[] {
                new Vector2(portalSize, -6f),
                new Vector2(portalSize, 16f),
                new Vector2(-portalSize, 16f),
                new Vector2(-portalSize, -6f)
            }
        };
        portalEast.RotationDegrees = new Vector3(0, -90f, 0);
        portalEast.Translation = new Vector3(ChunkWidth + 1f, 0, portalSize + 1f);

        return new[] {portalSouth, portalEast};
    }

    private MeshInstance CreateDebugPortalMesh(Portal portal) {
        var debugPortalMesh = new MeshInstance {Name = $"{portal.Name}DebugMesh", Mesh = new ArrayMesh()};
        var debugMeshArray = new Godot.Collections.Array();
        debugMeshArray.Resize((int) ArrayMesh.ArrayType.Max);
        debugMeshArray[(int) ArrayMesh.ArrayType.Vertex] = portal.Points;
        ((ArrayMesh)debugPortalMesh.Mesh).AddSurfaceFromArrays(Mesh.PrimitiveType.LineLoop, debugMeshArray);
        ((ArrayMesh)debugPortalMesh.Mesh).SurfaceSetMaterial(0, _debugPortalMaterial);
        debugPortalMesh.Translation = portal.Translation;
        debugPortalMesh.RotationDegrees = portal.RotationDegrees;
        debugPortalMesh.PortalMode = CullInstance.PortalModeEnum.Static;
        debugPortalMesh.IncludeInBound = true;
        return debugPortalMesh;
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