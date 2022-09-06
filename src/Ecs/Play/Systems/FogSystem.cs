using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Systems.Init;
using SatiRogue.lib.RelEcsGodot.src;
using SatiRogue.Tools;
namespace SatiRogue.Ecs.Play.Systems;

public class FogSystem : GdSystem {
   public override void Run() {
      var mapGenData = GetElement<MapGenData>();
      var fogMultiMeshes = GetElement<FogMultiMeshes>();

      foreach (var (_, gridPosition) in Query<Player, GridPositionComponent>()) {
         CalculateFov(gridPosition, mapGenData, fogMultiMeshes);
      }
   }

   static void CalculateFov(GridPositionComponent gridPositionComponent, MapGenData mapGenData, FogMultiMeshes fogMultiMeshes) {
      var offScreenCoords = new Vector3(-1000f, 1000f, -1000f);

      ShadowCast.ComputeVisibility(mapGenData, gridPositionComponent.Position, 11.0f);

      var maxWidth = mapGenData.GeneratorParameters.Width;
      var chunkWidth = mapGenData.GeneratorParameters.Width.Factors().GetMedian();

      while (mapGenData.CellsVisibilityChanged.Count > 0) {
         var position = mapGenData.CellsVisibilityChanged.Pop();
         var chunkId = SpatialMapSystem.GetChunkIdForPosition(position, chunkWidth, maxWidth);
         var localPos = position - InitFogSystem.GetChunkMinMaxCoords(chunkId, maxWidth + chunkWidth, chunkWidth)[0];
         var localId = (int) localPos.x + (int) localPos.z * chunkWidth;
         fogMultiMeshes[chunkId].Multimesh.SetInstanceTransform(localId, new Transform(Basis.Identity, offScreenCoords));
      }
   }
}