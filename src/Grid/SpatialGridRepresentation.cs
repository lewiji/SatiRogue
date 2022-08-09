using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;
using SatiRogue.scenes;
using SatiRogue.Tools;
namespace SatiRogue.Grid;

public partial class SpatialGridRepresentation : Spatial, IDependent {
   private static readonly Dictionary<CellType, Mesh> CellMeshResources = new() {
      {CellType.Floor, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8235.mesh")},
      {CellType.Stairs, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface6972.mesh")},
      {CellType.Wall, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7509.mesh")},
      {CellType.DoorClosed, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7081.mesh")},
      {CellType.DoorOpen, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8475.mesh")}
   };

   private static readonly Vector3 OffScreenCoords = new(-1000f, 1000f, -1000f);

   private readonly Array _cellTypes = Enum.GetValues(typeof(CellType));
   private readonly Mesh _fogMesh = GD.Load<Mesh>("res://scenes/ThreeDee/res/FogTileMesh.tres");
   private readonly List<MultiMeshInstance> _fogMultiMeshes = new();
   private int _chunkSize;
   private int _chunkWidth = 15;
   private bool _mapDataDirty = false;
   private int _maxWidth;

   [OnReadyGet("../", Export = true)] private ThreeDee? _threeDee;
   private int _totalChunks;
   [Dependency] private RuntimeMapNode RuntimeMapNode { get => this.DependOn<RuntimeMapNode>(); }

   [Export] private bool SaveDebugScene { get; set; }

   public void Loaded() {
      ConnectToGridGenerator();
   }

   [OnReady]
   private void DeferredCallToGridGeneratorSignal() {
      Logger.Debug("Connecting to map changed signal on next frame...");
      this.Depend();
   }

   private void ConnectToGridGenerator() {
      Logger.Debug("Connecting to map changed signal.");
      RuntimeMapNode.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(OnMapDataChanged));
      RuntimeMapNode.Connect(nameof(RuntimeMapNode.VisibilityChanged), this, nameof(OnVisibilityChanged));
   }

   private Vector3i[] GetChunkMinMaxCoords(int chunkId, int maxWidth) {
      var start = new Vector3i(chunkId * _chunkWidth % maxWidth, 0,
         Mathf.FloorToInt(chunkId * (float) _chunkWidth / maxWidth) * _chunkWidth);

      var end = start + new Vector3i(_chunkWidth, 0, _chunkWidth);
      var coords = new[] {start, end};

      return coords;
   }

   private bool ChunkPositionCondition(Cell c, IList<Vector3i> chunkCoords) {
      return c.Position.x >= chunkCoords[0].x && c.Position.x <= chunkCoords[1].x && c.Position.z >= chunkCoords[0].z
             && c.Position.z <= chunkCoords[1].z;
   }

   private int GetChunkIdForPosition(Vector3i position) {
      return position.x / _chunkWidth + position.z / _chunkWidth * ((_maxWidth + _chunkWidth) / _chunkWidth);
   }

   private async void OnVisibilityChanged(Vector3[] positions) {
      if (_totalChunks == 0) {
         await ToSignal(GetTree(), "idle_frame");
      }
      Logger.Info("Spatial visibility updating");

      foreach (var position in positions) {
         var chunkId = GetChunkIdForPosition(new Vector3i(position));
         var localPos = position - GetChunkMinMaxCoords(chunkId, _maxWidth + _chunkWidth)[0].ToVector3();
         var localId = (int) localPos.x + (int) localPos.z * _chunkWidth;
         _fogMultiMeshes[chunkId].Multimesh.SetInstanceTransform(localId, new Transform(Basis.Identity, OffScreenCoords));
      }
   }

   private async void OnMapDataChanged() {
      BuildMapData();
   }

   /*public override void _PhysicsProcess(float delta) {
      if (_mapDataDirty) {
         BuildMapData();
         _mapDataDirty = false;
      }
   }*/

   private void BuildMapData() {
      Logger.Debug("3d: Map data changed");

      foreach (var multiMeshInstance in _fogMultiMeshes) {
         multiMeshInstance.QueueFree();
      }
      _fogMultiMeshes.Clear();

      var cells = RuntimeMapNode.MapData?.Cells.ToArray();
      var mapParams = MapGenerator.GetParams().GetValueOrDefault();
      _maxWidth = mapParams.Width;
      _chunkWidth = mapParams.Width.Factors().GetMedian();
      _chunkSize = _chunkWidth * _chunkWidth;
      _totalChunks = Mathf.CeilToInt((mapParams.Width + _chunkWidth) * (mapParams.Height + _chunkWidth) / (float) _chunkSize);

      Logger.Info($"Chunk width: {_chunkWidth}");
      Logger.Info($"Max width: {_maxWidth}");
      Logger.Info($"{cells.Length} map cells total.");
      Logger.Info($"Total chunks: {_totalChunks}");

      for (var chunkId = 0; chunkId < _totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId, _maxWidth + _chunkWidth);

         if (cells == null) continue;
         var chunkCells = cells.Where(c => ChunkPositionCondition(c, chunkCoords)).ToArray();
         Logger.Debug($"Chunking: Taking {chunkCells.Length} cells");
         Logger.Debug("---");
         // Remove these cells from the enumeration
         cells = cells.Except(chunkCells).ToArray();

         Logger.Debug($"{cells.Length} cells remaining in map data");
         Logger.Debug($"Building chunk {chunkId}.");

         BuildChunk(chunkId, chunkCoords, chunkCells);
      }

      Logger.Info("Chunking finished.");
   }

   private void BuildChunk(int chunkId, Vector3i[] chunkCoords, Cell[] chunkCells) {
      var chunkRoom = new Room {
         Name = $"Chunk{chunkId}"
      };
      AddChild(chunkRoom);
      chunkRoom.Owner = this;
      chunkRoom.Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z);
      chunkRoom.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;

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
            CastShadow = GeometryInstance.ShadowCastingSetting.On,
            PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off
         };
         chunkRoom.AddChild(mmInst);
         mmInst.Owner = this;

         for (var i = 0; i < cellsOfThisTypeInChunk.Length; i++) {
            mmInst.Multimesh.SetInstanceTransform(i,
               new Transform(Basis.Identity, cellsOfThisTypeInChunk[i].Position.ToVector3() - chunkRoom.Translation));
         }
      }

      // Create Fog MultiMesh
      var fogMultiMeshInstance = new MultiMeshInstance {
         Multimesh = new MultiMesh {
            Mesh = _fogMesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
            InstanceCount = _chunkWidth * _chunkWidth
         },
         CastShadow = GeometryInstance.ShadowCastingSetting.Off,
         PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off
      };
      chunkRoom.AddChild(fogMultiMeshInstance);
      fogMultiMeshInstance.Owner = this;

      for (var i = 0; i < fogMultiMeshInstance.Multimesh.InstanceCount; i++) {
         var fogPosition = new Vector3(i % _chunkWidth, 0.618f, Mathf.FloorToInt(i / _chunkWidth));
         fogMultiMeshInstance.Multimesh.SetInstanceTransform(i, new Transform(Basis.Identity, fogPosition));
      }

      var collider = new StaticBody();
      var colShape = new CollisionShape();
      chunkRoom.AddChild(collider);
      collider.AddChild(colShape);
      colShape.Shape = new BoxShape {Extents = new Vector3(_chunkWidth - 0.5f, 0.2f, _chunkWidth - 0.5f)};
      collider.Translation = new Vector3(_chunkWidth, 0.1f, _chunkWidth);

      _fogMultiMeshes.Add(fogMultiMeshInstance);

      /*var debugText = (DebugSpatialText) _debugTextScene.Instance();
      debugText.Translation = new Vector3(ChunkWidth / 2, 1.5f, ChunkWidth / 2);
      chunkRoom.AddChild(debugText);
      debugText.Owner = this;
      debugText.SetText(chunkId.ToString());*/
   }

   private void SaveGeometryToScene() {
      var packed = new PackedScene();
      packed.Pack(this);
      ResourceSaver.Save("res://testgeometry.scn", packed);
   }

   private static Mesh? GetMeshResourceForCellType(CellType? cellType) {
      CellMeshResources.TryGetValue(cellType.GetValueOrDefault(), out var mesh);

      return mesh;
   }
}