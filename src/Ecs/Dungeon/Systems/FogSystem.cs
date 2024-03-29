using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Systems.Init;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class FogSystem : ISystem {
   
   static readonly Vector3 OffScreenCoords = new (-1000f, 1000f, -1000f);

   public void Run(World world) {
      var mapGenData = world.GetElement<MapGenData>();
      var fogMultiMeshes = world.GetElement<FogMultiMeshes>();

      foreach (var (_, gridPosition) in world.Query<Player, GridPositionComponent>().Build()) {
         CalculateFov(gridPosition, mapGenData, fogMultiMeshes);
      }
   }

   public static void CalculateFov(GridPositionComponent gridPositionComponent, MapGenData mapGenData, FogMultiMeshes fogMultiMeshes) {
      ShadowCast.ComputeVisibility(mapGenData, gridPositionComponent.Position, 11.5f);

      var maxWidth = mapGenData.GeneratorParameters.Width;
      var chunkWidth = mapGenData.GeneratorParameters.Width.Factors().GetMedian();

      while (mapGenData.CellsVisibilityChanged.Count > 0) {
         var position = mapGenData.CellsVisibilityChanged.Pop();
         var chunkId = SpatialMapSystem.GetChunkIdForPosition(position, chunkWidth, maxWidth);
         var localPos = position - InitFogSystem.GetChunkMinMaxCoords(chunkId, maxWidth + chunkWidth, chunkWidth)[0];
         var localId = (int) localPos.x + (int) localPos.z * chunkWidth;
         fogMultiMeshes.Instances[chunkId].Multimesh.SetInstanceColor(localId, new Color(0, 0, 0, 0f));
      }
   }
}