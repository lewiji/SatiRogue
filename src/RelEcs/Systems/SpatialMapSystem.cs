using Godot;
using RelEcs;
using SatiRogue.RelEcs.Components.MapGen;
using SatiRogue.Tools;
using Room = SatiRogue.RelEcs.Components.MapGen.Room;

namespace SatiRogue.RelEcs.Systems; 

public class SpatialMapSystem : GDSystem {
   private static PackedScene _roomScene = GD.Load<PackedScene>("res://scenes/ThreeDee/Room.tscn");
   public override void Run() {
      var mapData = GetElement<MapGenData>();

      foreach (Rect2 rect2 in mapData.GeneratorSpaces) {
         var box = _roomScene.Instance<MeshInstance>();
         ((CubeMesh) box.Mesh).Size = new Vector3(rect2.Size.x, 0.25f, rect2.Size.y);
         box.Translation = rect2.Position.ToVector3();
         GetElement<Nodes.Entities>().AddChild(box);
         Spawn(box).Add<Room>();
      }
   }
}