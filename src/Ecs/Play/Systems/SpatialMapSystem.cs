using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes;
using RelEcs;
using SatiRogue.Tools;
using Array = System.Array;

namespace SatiRogue.Ecs.Play.Systems; 

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

   public override void Run() {
      _mapGenData = GetElement<MapGenData>();
      _mapGeometry = GetElement<MapGeometry>();
      
      var maxWidth = _mapGenData.GeneratorParameters.Width;
      var chunkWidth = _mapGenData.GeneratorParameters.Width.Factors().GetMedian();
      var chunkSize = chunkWidth * chunkWidth;
      var totalChunks = Mathf.CeilToInt(
         (_mapGenData.GeneratorParameters.Width + chunkWidth) * 
         (_mapGenData.GeneratorParameters.Height + chunkWidth) / (float) chunkSize);

      BuildChunks(maxWidth, totalChunks, chunkWidth);
   }

   private void BuildChunks(int maxWidth, int totalChunks, int chunkWidth) {
      var cells = _mapGenData.IndexedCells.Values.ToArray();
      
      for (var chunkId = 0; chunkId < totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId,  chunkWidth, maxWidth + chunkWidth);
         if (cells == null) continue;
         // Remove these cells from the enumeration
         var chunkCells = cells.Where(c => ChunkPositionCondition(c, chunkCoords)).ToArray();
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
   
   private Vector3[] GetChunkMinMaxCoords(int chunkId, int chunkWidth, int maxWidth) {
      var start = new Vector3(
         chunkId * chunkWidth % maxWidth,
         0,
         Mathf.FloorToInt(chunkId * (float) chunkWidth / maxWidth) * chunkWidth);

      var end = start + new Vector3(chunkWidth, 0, chunkWidth);
      var coords = new[] {start, end};
      return coords;
   }
   
   private bool ChunkPositionCondition(Cell c, IList<Vector3> chunkCoords) {
      var position = c.Position.GetValueOrDefault();
      return position.x >= chunkCoords[0].x && position.x <= chunkCoords[1].x &&
             position.z >= chunkCoords[0].z && position.z <= chunkCoords[1].z;
   }
}