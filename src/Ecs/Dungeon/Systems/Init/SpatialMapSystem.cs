using System;
using System.Linq;
using Godot;
using Godot.Collections;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Resources;
using SatiRogue.Ecs.MapGenerator;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Tools;
using World = RelEcs.World;
using Array = System.Array;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpatialMapSystem : ISystem {
   public World World { get; set; } = null!;

   static readonly Dictionary<CellType, Mesh> CellMeshResources = new() {
      {CellType.Floor, GD.Load<Mesh>("res://resources/level_meshes/floor_tile_mesh.tres")},
      {CellType.Stairs, GD.Load<Mesh>("res://resources/level_meshes/stairs_mesh.tres")},
      {CellType.Wall, GD.Load<Mesh>("res://resources/level_meshes/wall_mesh.tres")}, {
         CellType.DoorClosed,
         GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface7081.mesh")
      }, {
         CellType.DoorOpen,
         GD.Load<Mesh>("res://scenes/ThreeDee/res_compressed/polySurface8475.mesh")
      }
   };

   static readonly Array<LevelMaterialSet> LevelMaterialSets = new() {
      GD.Load<LevelMaterialSet>("res://resources/level_material_sets/dungeon_0.tres"),
      GD.Load<LevelMaterialSet>("res://resources/level_material_sets/dungeon_1.tres")
   };

   static readonly PackedScene LightingScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/DungeonDirectionalLight.tscn");
   static readonly PackedScene FloorPlaneScene = GD.Load<PackedScene>("res://resources/props/FloorPlane.tscn");
   readonly Array _cellTypes = Enum.GetValues(typeof(CellType));

   MapGenData _mapGenData = null!;
   MapGeometry _mapGeometry = null!;

   public void Run() {
      _mapGenData = World.GetElement<MapGenData>();
      _mapGeometry = World.GetElement<MapGeometry>();

      var maxWidth = _mapGenData.GeneratorParameters.Width;
      var chunkWidth = _mapGenData.GeneratorParameters.Width.Factors().GetMedian();
      var chunkSize = chunkWidth * chunkWidth;

      var totalChunks = Mathf.CeilToInt((_mapGenData.GeneratorParameters.Width + chunkWidth)
         * (_mapGenData.GeneratorParameters.Height + chunkWidth) / (float) chunkSize);

      Logger.Info("Building chunks");
      _mapGeometry.AddChild(FloorPlaneScene.Instance());
      _mapGeometry.AddChild(LightingScene.Instance());
      ChooseLevelMaterialSet();
      BuildChunks(maxWidth, totalChunks, chunkWidth);
   }

   void ChooseLevelMaterialSet() {
      var idx = Mathf.RoundToInt((float) GD.RandRange(0, LevelMaterialSets.Count - 1));
      var levelMatSet = LevelMaterialSets[idx];
      Logger.Info($"Chose {levelMatSet} Level Material Set.");
      CellMeshResources[CellType.Floor].SurfaceSetMaterial(0, levelMatSet.FloorMaterial);
      CellMeshResources[CellType.Wall].SurfaceSetMaterial(0, levelMatSet.WallMaterial);
      CellMeshResources[CellType.Stairs].SurfaceSetMaterial(0, levelMatSet.StairsMaterial);
   }

   void BuildChunks(int maxWidth, int totalChunks, int chunkWidth) {
      var cells = _mapGenData.IndexedCells.Values.ToArray();

      var chunkCells = new Cell[chunkWidth * chunkWidth];

      for (var chunkId = 0; chunkId < totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId, chunkWidth, maxWidth + chunkWidth);

         if (cells == null)
            continue;
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
      var chunkRoom = new Spatial {Name = $"Chunk{chunkId}"};
      _mapGeometry.AddChild(chunkRoom);
      chunkRoom.Owner = _mapGeometry;
      chunkRoom.Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z);

      // Create MultiMesh for each cell type in this chunk data
      foreach (CellType cellType in _cellTypes) {
         var cellsOfThisTypeInChunk = chunkCells.Where(cell => cell != null && cell.Type == cellType).ToArray();
         var meshForCellType = GetMeshResourceForCellType(cellType);

         if (meshForCellType == null)
            continue;

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

   public static int GetChunkIdForPosition(Vector3 position, int chunkWidth, int maxWidth) {
      return (int) position.x / chunkWidth + (int) position.z / chunkWidth * ((maxWidth + chunkWidth) / chunkWidth);
   }
}