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

   static readonly Mesh CellMesh = GD.Load<Mesh>("res://resources/level_meshes/1_1_cube_Cube.mesh");


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
      //ChooseLevelMaterialSet();
      BuildChunks(maxWidth, totalChunks, chunkWidth);
   }

   void ChooseLevelMaterialSet() {
      /*var idx = Mathf.RoundToInt((float) GD.RandRange(0, LevelMaterialSets.Count - 1));
      var levelMatSet = LevelMaterialSets[idx];
      Logger.Info($"Chose {levelMatSet} Level Material Set.");
      CellMeshResources[CellType.Floor].SurfaceSetMaterial(0, levelMatSet.FloorMaterial);
      CellMeshResources[CellType.Wall].SurfaceSetMaterial(0, levelMatSet.WallMaterial);
      CellMeshResources[CellType.Stairs].SurfaceSetMaterial(0, levelMatSet.StairsMaterial);*/
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
      
      
      var mmInst = new MultiMeshInstance {
         Multimesh = new MultiMesh {
            Mesh = CellMesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
            CustomDataFormat = MultiMesh.CustomDataFormatEnum.Data8bit,
            InstanceCount = chunkCells.GetUpperBound(0)
         },
         CastShadow = GeometryInstance.ShadowCastingSetting.On,
         PhysicsInterpolationMode = Node.PhysicsInterpolationModeEnum.Off
      };
      chunkRoom.AddChild(mmInst);
      mmInst.Owner = _mapGeometry;

      var instanceId = 0;
      // Create MultiMesh for each cell type in this chunk data
      foreach (var chunkCell in chunkCells) {
         if (chunkCell is not { } cell || cell.Type == CellType.Void) continue;
         SetTile(mmInst.Multimesh, instanceId++, cell, chunkRoom);
      }
   }

   void SetTile(MultiMesh mMesh, int instanceId, Cell cell, Spatial room) {
      if (GetCellCustomDataForCellType(cell.Type) is not { } color) return;
      mMesh.SetInstanceTransform(instanceId, new Transform(Basis.Identity, cell.Position - room.Translation));
      mMesh.SetInstanceCustomData(instanceId, color);
   }

   Color? GetCellCustomDataForCellType(CellType? cellType) {
      switch (cellType) {
         case CellType.Floor:
            return Color.Color8(0, 0, 0, 0);
         case CellType.Wall:
            return Color.Color8(1, 0, 0, 0);
         case CellType.DoorClosed: 
            return Color.Color8(2, 0, 0, 0);
         case CellType.DoorOpen: 
            return Color.Color8(3, 0, 0, 0);
         case CellType.Stairs:
            return Color.Color8(4, 0, 0, 0);
         case CellType.Void: break;
         case null: break;
         default: throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
      }
      return null;
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