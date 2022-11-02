using System;
using System.Collections.Generic;
using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes;

[Tool]
public class RoomMultiMesh : MultiMeshInstance {
   public enum RoomTileType { Floor, Wall }
   public struct TileData {
      public TileData(RoomTileType tileType, Vector3 translation) : this() {
         TileType = tileType;
         Translation = translation;
      }
      public RoomTileType TileType { get; set; }
      public Vector3 Translation { get; set; }
      public int TileIndexOffset { get; set; } = 0;
   }

   [Export] int Width {
      get => _width;
      set {
         _width = value;
         CallDeferred(nameof(CreateRoom));
      }
   }
   [Export] int Height {
      get => _height;
      set {
         _height = value;
         CallDeferred(nameof(CreateRoom));
      }
   }
   [Export] int FloorTile {
      get => _floorTile;
      set {
         _floorTile = value;
         CallDeferred(nameof(CreateRoom));
      }
   }
   [Export] int WallTile {
      get => _wallTile;
      set {
         _wallTile = value;
         CallDeferred(nameof(CreateRoom));
      }
   }

   List<TileData> _tileData = new ();
   int _width = 5;
   int _height = 5;
   int _floorTile = 0;
   int _wallTile = 1;

   public override void _EnterTree() {
      Multimesh.InstanceCount = 0;
      Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
      Multimesh.CustomDataFormat = MultiMesh.CustomDataFormatEnum.Data8bit;
   }

   public void CreateRoom() {
      _tileData.Clear();
      EnumerateTileTranslations();
      Multimesh.InstanceCount = _tileData.Count;
      InstanceMeshes();
   }

   void EnumerateTileTranslations() {
      for (var y = 0; y < Height; y++) {
         for (var x = 0; x < Width; x++) {
            AddFloorTileTranslation(x, y);
         }
      }

      for (var wallEastWest = 0; wallEastWest < Height; wallEastWest++) {
         AddWallTileTranslation(-1, wallEastWest);
         AddWallTileTranslation(Width, wallEastWest);
      }

      for (var wallNorthSouth = -1; wallNorthSouth <= Width; wallNorthSouth++) {
         AddWallTileTranslation(wallNorthSouth, -1);
         AddWallTileTranslation(wallNorthSouth, Height);
      }
   }

   void AddFloorTileTranslation(int x, int y, float height = 0) {
      _tileData.Add(new TileData(tileType: RoomTileType.Floor, translation: new Vector3(x, height, y)));
   }

   void AddWallTileTranslation(int x, int y, float height = 1) {
      _tileData.Add(new TileData(tileType: RoomTileType.Wall, translation: new Vector3(x, height, y)));
   }

   void InstanceMeshes() {
      for (var index = 0; index < _tileData.Count; index++) {
         var tileData = _tileData[index];
         SetInstance(index, tileData);
      }
   }

   void SetInstance(int instanceId, TileData tileData) {
      Multimesh.SetInstanceTransform(instanceId, new Transform(Basis.Identity, tileData.Translation));
      switch (tileData.TileType) {
         case RoomTileType.Floor:
            SetFloorTile(instanceId);
            break;
         case RoomTileType.Wall: 
            SetWallTile(instanceId);
            break;
         default: throw new ArgumentOutOfRangeException();
      }
   }

   void SetWallTile(int instanceId, int tileOffset = 0) {
      Multimesh.SetInstanceCustomData(instanceId, Color.Color8((byte) (WallTile + tileOffset), 0, 0, 0));
   }

   void SetFloorTile(int instanceId, int tileOffset = 0) {
      Multimesh.SetInstanceCustomData(instanceId, Color.Color8((byte) (FloorTile + tileOffset), 0, 0, 0));
   }
}