using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
using SatiRogue.Tools;
using Array = System.Array;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpatialMapSystem : GdSystem {
   static readonly Godot.Collections.Dictionary<CellType, Mesh> CellMeshResources = new() {
      {CellType.Floor, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8235.mesh")},
      {CellType.Stairs, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface6972.mesh")},
      {CellType.Wall, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7509.mesh")},
      {CellType.DoorClosed, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7081.mesh")},
      {CellType.DoorOpen, GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8475.mesh")}
   };

   static readonly PackedScene FloorPlaneScene = GD.Load<PackedScene>("res://resources/props/FloorPlane.tscn");
   static readonly Material WallShadows = GD.Load<Material>("res://assets/overworld/WallMatShadows.tres");
   readonly Array _cellTypes = Enum.GetValues(typeof(CellType));

   MapGenData _mapGenData = null!;
   MapGeometry _mapGeometry = null!;

   public override void Run() {
      _mapGenData = GetElement<MapGenData>();
      _mapGeometry = GetElement<MapGeometry>();

      var maxWidth = _mapGenData.GeneratorParameters.Width;
      var chunkWidth = _mapGenData.GeneratorParameters.Width.Factors().GetMedian();
      var chunkSize = chunkWidth * chunkWidth;

      var totalChunks = Mathf.CeilToInt((_mapGenData.GeneratorParameters.Width + chunkWidth)
         * (_mapGenData.GeneratorParameters.Height + chunkWidth) / (float) chunkSize);

      Logger.Info("Building chunks");
      _mapGeometry.AddChild(FloorPlaneScene.Instance());
      BuildChunks(maxWidth, totalChunks, chunkWidth);
   }

   void BuildChunks(int maxWidth, int totalChunks, int chunkWidth) {
      var cells = _mapGenData.IndexedCells.Values.ToArray();

      var chunkCells = new Cell[chunkWidth * chunkWidth];

      for (var chunkId = 0; chunkId < totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId, chunkWidth, maxWidth + chunkWidth);
         if (cells == null) continue;
         var cellCount = 0;

         // Remove these cells from the enumeration
         for (var y = chunkCoords[0].z; y < chunkCoords[1].z; y++) {
            for (var x = chunkCoords[0].x; x < chunkCoords[1].x; x++) {
               var id = IdCalculator.IdFromVec3(new Vector3(x, 0, y));
               var found = _mapGenData.IndexedCells.TryGetValue(id, out var cell);

               if (found) {
                  chunkCells[cellCount] = cell!;
                  cellCount++;
               }
            }
         }

         BuildChunk(chunkId, chunkCoords, chunkCells);
         Array.Clear(chunkCells, 0, cellCount);
      }
   }

   void BuildChunk(int chunkId, Vector3[] chunkCoords, Cell[] chunkCells) {
      var chunkRoom = new Spatial {
         Name = $"Chunk{chunkId}"
      };
      _mapGeometry.AddChild(chunkRoom);
      chunkRoom.Owner = _mapGeometry;
      chunkRoom.Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z);

      // Create MultiMesh for each cell type in this chunk data
      foreach (CellType cellType in _cellTypes) {
         var cellsOfThisTypeInChunk = chunkCells.Where(cell => cell != null && cell.Type == cellType).ToArray();
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
               new Transform(Basis.Identity, cellsOfThisTypeInChunk[i].Position - chunkRoom.Translation));
         }

         if (cellType is CellType.Wall) {
            mmInst.Multimesh.ResourceLocalToScene = true;
            var wallsShadows = (MultiMeshInstance) mmInst.Duplicate();
            wallsShadows.MaterialOverride = WallShadows;
            wallsShadows.CastShadow = GeometryInstance.ShadowCastingSetting.ShadowsOnly;
            chunkRoom.AddChild(wallsShadows);
         }
      }
   }

   static Mesh? GetMeshResourceForCellType(CellType? cellType) {
      CellMeshResources.TryGetValue(cellType.GetValueOrDefault(), out var mesh);
      return mesh;
   }

   Vector3[] GetChunkMinMaxCoords(int chunkId, int chunkWidth, int maxWidth) {
      var start = new Vector3(chunkId * chunkWidth % maxWidth, 0, Mathf.FloorToInt(chunkId * (float) chunkWidth / maxWidth) * chunkWidth);

      var end = start + new Vector3(chunkWidth, 0, chunkWidth);
      var coords = new[] {start, end};
      return coords;
   }
}