using Godot;
using SatiRogue.Grid;
using SatiRogue.MathUtils;

namespace SatiRogue.Commands.MapGen;

public class MapGenConnectPathfindingPoints : MapGenCommand {
   private static readonly Vector3i[] Offsets = {
      Vector3i.Back, Vector3i.Forward, Vector3i.Left, Vector3i.Right, Vector3i.Back + Vector3i.Left,
      Vector3i.Back + Vector3i.Right, Vector3i.Forward + Vector3i.Left, Vector3i.Forward + Vector3i.Right
   };

   public MapGenConnectPathfindingPoints(MapData mapData) : base(mapData) { }

   public override Error Execute() {
      var points = MapData.AStar.GetPoints();
      foreach (int point in points)
      foreach (var offset in Offsets) {
         var neighbourVec = new Vector3i(MapData.AStar.GetPointPosition(point)) + offset;
         if (MapData.CellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(neighbourVec), out var neighbourId))
            MapData.AStar.ConnectPoints(point, neighbourId);
      }

      return Error.Ok;
   }
}