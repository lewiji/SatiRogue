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
   

   static readonly Mesh CellMesh = GD.Load<Mesh>("res://resources/level_meshes/dungeon_voxel.mesh");

   MapGenData _mapGenData = null!;
   MapGeometry _mapGeometry = null!;

   record LevelTileIndexSet
   {
	   public Dictionary<CellType, int> CellTypeTileIndices { get; set; } = new();
   }

   readonly LevelTileIndexSet[] _levelTiles = new[] {
	   new LevelTileIndexSet {
		   CellTypeTileIndices = {
			   {CellType.Wall, 0},
			   {CellType.Floor, 1}
		   }
	   },
	   new LevelTileIndexSet {
		   CellTypeTileIndices = {
			   {CellType.Wall, 2},
			   {CellType.Floor, 3}
		   }
	   },
	   new LevelTileIndexSet {
		   CellTypeTileIndices = {
			   {CellType.Wall, 4},
			   {CellType.Floor, 5}
		   }
	   },
	   new LevelTileIndexSet {
		   CellTypeTileIndices = {
			   {CellType.Wall, 6},
			   {CellType.Floor, 7}
		   }
	   },
   };

   public void Run(World world) {
      _mapGenData = world.GetElement<MapGenData>();
      _mapGeometry = world.GetElement<MapGeometry>();

      var maxWidth = _mapGenData.GeneratorParameters.Width;
      var chunkWidth = _mapGenData.GeneratorParameters.Width.Factors().GetMedian();
      var chunkSize = chunkWidth * chunkWidth;

      var totalChunks = Mathf.CeilToInt((_mapGenData.GeneratorParameters.Width + chunkWidth)
         * (_mapGenData.GeneratorParameters.Height + chunkWidth) / (float) chunkSize);

      Logger.Info("Building chunks");
      ChooseLevelMaterialSet();
      BuildChunks(maxWidth, totalChunks, chunkWidth, ChooseLevelMaterialSet());
   }

   LevelTileIndexSet ChooseLevelMaterialSet() {
      var idx = Mathf.RoundToInt((float) GD.RandRange(0, _levelTiles.Length - 1));
      return _levelTiles[idx];
   }

   void BuildChunks(int maxWidth, int totalChunks, int chunkWidth, LevelTileIndexSet levelTiles) {
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

         BuildChunk(chunkId, chunkCoords, chunkCells, levelTiles);
         Array.Clear(chunkCells, 0, cellCount);
      }
   }

   void BuildChunk(int chunkId, Vector3[] chunkCoords, Cell[] chunkCells, LevelTileIndexSet levelTiles) {
      var chunkRoom = new Spatial {Name = $"Chunk{chunkId}"};
      _mapGeometry.AddChild(chunkRoom);
      chunkRoom.Owner = _mapGeometry;
      chunkRoom.Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z);
      
      
      var mmInst = new MultiMeshInstance {
         Multimesh = new MultiMesh {
            Mesh = CellMesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
            CustomDataFormat = MultiMesh.CustomDataFormatEnum.Data8bit,
            InstanceCount = chunkCells.Count(c => c is { Type: CellType.Wall or CellType.Floor })
         },
         CastShadow = GeometryInstance.ShadowCastingSetting.On,
         PhysicsInterpolationMode = Node.PhysicsInterpolationModeEnum.Off,
         UseInBakedLight = true,
         GenerateLightmap = true,
      };
      chunkRoom.AddChild(mmInst);
      mmInst.Owner = _mapGeometry;

      var instanceId = 0;
      // Create MultiMesh for each cell type in this chunk data
      foreach (var chunkCell in chunkCells) {
	      if (chunkCell is not {Type: (CellType.Wall or CellType.Floor)}) continue;
	      SetTile(mmInst.Multimesh, instanceId, chunkCell, chunkRoom, levelTiles);
	      instanceId += 1;
      }
   }

   void SetTile(MultiMesh mMesh, int instanceId, Cell cell, Spatial room, LevelTileIndexSet levelTiles) {
      if (GetCellCustomDataForCellType(cell.Type, levelTiles) is not { } color) return;
      mMesh.SetInstanceTransform(instanceId, new Transform(Basis.Identity, 
         GetTranslationOffsetForCellType(cell.Type) + cell.Position - room.Translation));
      mMesh.SetInstanceCustomData(instanceId, color);
   }

   Color? GetCellCustomDataForCellType(CellType? cellType, LevelTileIndexSet levelTiles) {
	   if (levelTiles.CellTypeTileIndices.TryGetValue(cellType.GetValueOrDefault(), out var value)) {
		   return Color.Color8((byte) value, 0, 0, 0);
	   }
	   return null;
   }
   
   Vector3 GetTranslationOffsetForCellType(CellType? cellType) {
      switch (cellType) {
         case CellType.Wall:
            return new Vector3(0, 0, 0);
         case CellType.Floor:
         case CellType.DoorClosed: 
         case CellType.DoorOpen: 
         case CellType.Stairs:
         case CellType.Void:
         case null: return new Vector3(0, -1, 0);
         default: throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
      }
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