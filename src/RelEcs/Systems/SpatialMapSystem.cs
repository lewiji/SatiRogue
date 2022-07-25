using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.RelEcs.Components.MapGen;
using SatiRogue.RelEcs.Nodes;
using SatiRogue.Tools;
using Array = System.Array;
using Room = SatiRogue.RelEcs.Components.MapGen.Room;

namespace SatiRogue.RelEcs.Systems; 

public class SpatialMapSystem : GDSystem {
   private readonly Array _cellTypes = Enum.GetValues(typeof(Grid.CellType));
   private static readonly Godot.Collections.Dictionary<CellType, Mesh> CellMeshResources = new() {
      {CellType.Floor, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8235.mesh")},
      {CellType.Stairs, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface6972.mesh")},
      {CellType.Wall, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7509.mesh")},
      {CellType.DoorClosed, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7081.mesh")},
      {CellType.DoorOpen, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8475.mesh")}
   };
   
   private MapGenData _mapGenData = null!;
   private MapGeometry _mapGeometry = null!;
   private int _chunkWidth;
   private int _chunkSize;
   private int _maxWidth;
   private int _totalChunks;

   public override void Run() {
      _mapGenData = GetElement<MapGenData>();
      _mapGeometry = GetElement<MapGeometry>();
      var generatorParameters = _mapGenData.GeneratorParameters;
      _maxWidth = generatorParameters.Width;
      _chunkWidth = generatorParameters.Width.Factors().GetMedian();
      _chunkSize = _chunkWidth * _chunkWidth;
      _totalChunks = Mathf.CeilToInt((generatorParameters.Width + _chunkWidth) * (generatorParameters.Height + _chunkWidth) / (float) _chunkSize);

      var cells = _mapGenData.IndexedCells.Values.ToArray();

      for (var chunkId = 0; chunkId < _totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId, _maxWidth + _chunkWidth);
         if (cells == null) continue;
         var chunkCells = cells.Where(c => ChunkPositionCondition(c, chunkCoords)).ToArray();
         Logger.Debug($"Chunking: Taking {chunkCells.Length} cells");
         Logger.Debug("---");
         // Remove these cells from the enumeration
         cells = cells.Except(chunkCells).ToArray();

         BuildChunk(chunkId, chunkCoords, chunkCells);
      }
   }
   
   private void BuildChunk(int chunkId, Vector3[] chunkCoords, Cell[] chunkCells) {
      var chunkRoom = new Spatial() {
         Name = $"Chunk{chunkId}"
      };
      _mapGeometry.AddChild(chunkRoom);
      chunkRoom.Owner = _mapGeometry;
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
            CastShadow = GeometryInstance.ShadowCastingSetting.On,
            PhysicsInterpolationMode = Node.PhysicsInterpolationModeEnum.Off
         };
         chunkRoom.AddChild(mmInst);
         mmInst.Owner = _mapGeometry;

         for (var i = 0; i < cellsOfThisTypeInChunk.Length; i++) {
            mmInst.Multimesh.SetInstanceTransform(i,
               new Transform(Basis.Identity, cellsOfThisTypeInChunk[i].Position.GetValueOrDefault() - chunkRoom.Translation));
         }
      }
   }

   private static Mesh? GetMeshResourceForCellType(CellType? cellType) {
      CellMeshResources.TryGetValue(cellType.GetValueOrDefault(), out var mesh);
      return mesh;
   }
   
   private Vector3[] GetChunkMinMaxCoords(int chunkId, int maxWidth) {
      var start = new Vector3(
         chunkId * _chunkWidth % maxWidth,
         0,
         Mathf.FloorToInt(chunkId * (float) _chunkWidth / maxWidth) * _chunkWidth);

      var end = start + new Vector3(_chunkWidth, 0, _chunkWidth);
      var coords = new[] {start, end};
      return coords;
   }
   
   private bool ChunkPositionCondition(Cell c, IList<Vector3> chunkCoords) {
      var position = c.Position.GetValueOrDefault();
      return position.x >= chunkCoords[0].x && position.x <= chunkCoords[1].x &&
             position.z >= chunkCoords[0].z && position.z <= chunkCoords[1].z;
   }
}