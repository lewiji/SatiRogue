using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Systems.Init;
using SatiRogue.MathUtils;
using SatiRogue.Tools;
namespace SatiRogue.Ecs.Play.Systems;

public class FogSystem : GDSystem {
   public override void Run() {
      var mapGenData = GetElement<MapGenData>();
      var fogMultiMeshes = GetElement<FogMultiMeshes>();

      foreach (var (player, gridPosition) in Query<Nodes.Actors.Player, GridPositionComponent>()) {
         CalculateFov(gridPosition, mapGenData, fogMultiMeshes);
      }
   }

   public static void CalculateFov(GridPositionComponent gridPositionComponent, MapGenData mapGenData, FogMultiMeshes fogMultiMeshes) {
      var offScreenCoords = new Vector3(-1000f, 1000f, -1000f);

      ShadowCast.ComputeVisibility(mapGenData, gridPositionComponent.Position, 11.0f);

      var maxWidth = mapGenData.GeneratorParameters.Width;
      var chunkWidth = mapGenData.GeneratorParameters.Width.Factors().GetMedian();

      foreach (var position in mapGenData.CellsVisibilityChanged) {
         Logger.Info("Spatial visibility updating");
         var chunkId = GetChunkIdForPosition(new Vector3i(position), chunkWidth, maxWidth);
         var localPos = position - InitFogSystem.GetChunkMinMaxCoords(chunkId, maxWidth + chunkWidth, chunkWidth)[0];
         var localId = (int) localPos.x + (int) localPos.z * chunkWidth;
         fogMultiMeshes[chunkId].Multimesh.SetInstanceTransform(localId, new Transform(Basis.Identity, offScreenCoords));
      }
   }

   private static int GetChunkIdForPosition(Vector3i position, int chunkWidth, int maxWidth) {
      return position.x / chunkWidth + position.z / chunkWidth * ((maxWidth + chunkWidth) / chunkWidth);
   }
}