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
   private static readonly Vector3 OffScreenCoords = new(-1000f, 1000f, -1000f);

   public override void Run() {
      var mapGenData = GetElement<MapGenData>();

      foreach (var (player, gridPosition) in Query<Nodes.Actors.Player, GridPositionComponent>()) {
         ShadowCast.ComputeVisibility(mapGenData, gridPosition.Position, 11.0f);

         var maxWidth = mapGenData.GeneratorParameters.Width;
         var chunkWidth = mapGenData.GeneratorParameters.Width.Factors().GetMedian();
         var fogMultiMeshes = GetElement<FogMultiMeshes>();

         foreach (var position in mapGenData.CellsVisibilityChanged) {
            Logger.Info("Spatial visibility updating");
            var chunkId = GetChunkIdForPosition(new Vector3i(position), chunkWidth, maxWidth);
            var localPos = position - InitFogSystem.GetChunkMinMaxCoords(chunkId, maxWidth + chunkWidth, chunkWidth)[0];
            var localId = (int) localPos.x + (int) localPos.z * chunkWidth;
            fogMultiMeshes[chunkId].Multimesh.SetInstanceTransform(localId, new Transform(Basis.Identity, OffScreenCoords));
         }
      }
   }

   private int GetChunkIdForPosition(Vector3i position, int chunkWidth, int maxWidth) {
      return position.x / chunkWidth + position.z / chunkWidth * ((maxWidth + chunkWidth) / chunkWidth);
   }
}