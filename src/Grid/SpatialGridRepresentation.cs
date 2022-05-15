using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Math;
using SatiRogue.scenes;
using SatiRogue.scenes.Debug;
using Array = System.Array;

namespace SatiRogue.Grid;

public partial class SpatialGridRepresentation : Spatial {
   private readonly Array _cellTypes = Enum.GetValues(typeof(CellType));
   private readonly PackedScene _debugTextScene = GD.Load<PackedScene>("res://scenes/Debug/DebugSpatialText.tscn");
   private readonly Mesh _fogMesh = GD.Load<Mesh>("res://scenes/ThreeDee/res/FogTileMesh.tres");
   private int _chunkSize;
   private readonly List<Spatial> _chunkSpatials = new();
   private readonly List<MultiMeshInstance> _fogMultiMeshes = new();
   private int _maxWidth;

   [OnReadyGet("../", Export = true)] private ThreeDee? _threeDee;
   private int _totalChunks;
   private readonly int ChunkWidth = 25;

   [Export] private bool SaveDebugScene { get; set; }

   [OnReady]
   private void DeferredCallToGridGeneratorSignal() {
      Logger.Debug("Connecting to map changed signal on next frame...");
      CallDeferred(nameof(ConnectToGridGenerator));
   }

   private void ConnectToGridGenerator() {
      Logger.Debug("Connecting to map changed signal.");
      var gridGenerator = _threeDee!.GridGenerator;
      gridGenerator?.Connect(nameof(MapGenerator.MapChanged), this, nameof(OnMapDataChanged));
      gridGenerator?.Connect(nameof(MapGenerator.VisibilityChanged), this, nameof(OnVisibilityChanged));
   }

   private Vector3i[] GetChunkMinMaxCoords(int chunkId, int maxWidth) {
      var start = new Vector3i(
         chunkId * ChunkWidth % maxWidth,
         0,
         Mathf.FloorToInt(chunkId * (float) ChunkWidth / maxWidth) * ChunkWidth);

      var end = start + new Vector3i(ChunkWidth, 0, ChunkWidth);
      var coords = new[] {start, end};
      return coords;
   }

   private bool ChunkPositionCondition(Cell c, IList<Vector3i> chunkCoords) {
      return c.Position.x >= chunkCoords[0].x && c.Position.x <= chunkCoords[1].x &&
             c.Position.z >= chunkCoords[0].z && c.Position.z <= chunkCoords[1].z;
   }

   private int GetChunkIdForPosition(Vector3i position) {
      return position.x / ChunkWidth + position.z / ChunkWidth * ((_maxWidth + ChunkWidth) / ChunkWidth);
   }

   private void OnVisibilityChanged(Vector3[] positions) {
      Logger.Info("Spatial visibility updating");
      foreach (var position in positions) {
         var chunkId = GetChunkIdForPosition(new Vector3i(position));
         var localPos = position - GetChunkMinMaxCoords(chunkId, _maxWidth + ChunkWidth)[0].ToVector3();
         var localId = (int) localPos.x + (int) localPos.z * ChunkWidth;
         _fogMultiMeshes[chunkId].Multimesh.SetInstanceTransform(localId, new Transform(Basis.Identity, Vector3.Down));
      }
   }

   private void OnMapDataChanged() {
      Logger.Debug("3d: Map data changed");

      var cells = MapGenerator._mapData.Cells.ToList();
      var mapParams = MapGenerator.GetParams();
      _maxWidth = mapParams["Width"];
      _chunkSize = ChunkWidth * ChunkWidth;
      _totalChunks = Mathf.CeilToInt((mapParams["Width"] + ChunkWidth) * (mapParams["Height"] + ChunkWidth) / (float) _chunkSize);

      Logger.Info($"Chunk width: {ChunkWidth}");
      Logger.Info($"Max width: {_maxWidth}");
      Logger.Info($"{cells.Count} map cells total.");
      Logger.Info($"Total chunks: {_totalChunks}");

      for (var chunkId = 0; chunkId < _totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId, _maxWidth + ChunkWidth);
         if (cells == null) continue;
         var chunkCells = cells.Where(c => ChunkPositionCondition(c, chunkCoords)).ToList();
         Logger.Debug($"Chunking: Taking {chunkCells.Count} cells");
         Logger.Debug("---");
         // Remove these cells from the enumeration
         cells = cells.Except(chunkCells).ToList();

         Logger.Debug($"{cells.Count} cells remaining in map data");
         Logger.Debug($"Building chunk {chunkId}.");

         BuildChunk(chunkId, chunkCoords, chunkCells);
      }

      foreach (var entityData in EntityRegistry.EntityList)
         if (entityData.Value is EnemyData enemyData) {
            var tilePosition = enemyData.GridPosition;
            var enemyNode = new Entity3D(enemyData) {
               Translation = tilePosition.ToVector3()
            };
            var sprite = new AnimatedSprite3D {
               Frames = EnemyResourceLocator.GetResource<SpriteFrames>(enemyData.EnemyType),
               MaterialOverride = EnemyResourceLocator.GetResource<Material>(enemyData.EnemyType),
               Playing = true,
               Animation = "idle",
               Centered = true,
               PixelSize = 0.05f,
               RotationDegrees = new Vector3(-33f, 0, 0)
            };
            if (sprite.Frames != null) {
               var firstFrameTexture = (Texture)((Godot.Collections.Array)((Dictionary)sprite.Frames.Animations[0])["frames"])[0];
               sprite.Translation = new Vector3(firstFrameTexture.GetWidth() / 2f, firstFrameTexture.GetHeight() / 2f, 0) * sprite.PixelSize;
            }
            enemyNode.AddChild(sprite);
            _threeDee?.EnemiesSpatial?.AddChild(enemyNode);
         }

      Logger.Info("Chunking finished.");

      if (SaveDebugScene) SaveGeometryToScene();
   }

   private void BuildChunk(int chunkId, Vector3i[] chunkCoords, List<Cell> chunkCells) {
      var chunkRoom = new Room {
         Name = $"Chunk{chunkId}"
      };
      AddChild(chunkRoom);
      chunkRoom.Owner = this;
      chunkRoom.Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z);

      // Create MultiMesh for each cell type in this chunk data
      foreach (CellType cellType in _cellTypes) {
         var cellsOfThisTypeInChunk = chunkCells.Where(cell => cell.Type == cellType).ToArray();
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


         for (var i = 0; i < cellsOfThisTypeInChunk.Length; i++)
            mmInst.Multimesh.SetInstanceTransform(i,
               new Transform(Basis.Identity, cellsOfThisTypeInChunk[i].Position.ToVector3() - chunkRoom.Translation));
      }

      // Create Fog MultiMesh
      var fogMultiMeshInstance = new MultiMeshInstance {
         Multimesh = new MultiMesh {
            Mesh = _fogMesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
            InstanceCount = ChunkWidth * ChunkWidth
         },
         CastShadow = GeometryInstance.ShadowCastingSetting.Off
      };
      chunkRoom.AddChild(fogMultiMeshInstance);
      fogMultiMeshInstance.Owner = this;
      for (var i = 0; i < fogMultiMeshInstance.Multimesh.InstanceCount; i++) {
         var fogPosition = new Vector3(
            i % ChunkWidth,
            0.618f,
            Mathf.FloorToInt(i / ChunkWidth));
         fogMultiMeshInstance.Multimesh.SetInstanceTransform(i, new Transform(Basis.Identity, fogPosition));
      }

      _fogMultiMeshes.Add(fogMultiMeshInstance);
      _chunkSpatials.Add(chunkRoom);

      var debugText = (DebugSpatialText) _debugTextScene.Instance();
      debugText.Translation = new Vector3(ChunkWidth / 2, 1.5f, ChunkWidth / 2);
      chunkRoom.AddChild(debugText);
      debugText.Owner = this;
      debugText.SetText(chunkId.ToString());
   }

   private void SaveGeometryToScene() {
      var packed = new PackedScene();
      packed.Pack(this);
      ResourceSaver.Save("res://testgeometry.scn", packed);
   }

   private static Mesh? GetMeshResourceForCellType(CellType? cellType) {
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