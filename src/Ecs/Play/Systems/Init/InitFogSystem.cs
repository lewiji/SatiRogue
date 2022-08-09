using System.Collections.Generic;
using System.Linq;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.Tools;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class FogMultiMeshes : List<MultiMeshInstance> { }

public class InitFogSystem : GDSystem {
   private readonly Mesh _fogMesh = GD.Load<Mesh>("res://scenes/ThreeDee/res/FogTileMesh.tres");
   private readonly FogMultiMeshes _fogMultiMeshes = new();
   private MapGeometry? _mapGeometry;

   public override void Run() {
      AddElement(_fogMultiMeshes);
      var mapGenData = GetElement<MapGenData>();
      var cells = mapGenData.IndexedCells.Values.ToArray();

      foreach (var mmInst in _fogMultiMeshes) mmInst.QueueFree();
      _fogMultiMeshes.Clear();

      var maxWidth = mapGenData.GeneratorParameters.Width;
      var chunkWidth = mapGenData.GeneratorParameters.Width.Factors().GetMedian();
      var chunkSize = chunkWidth * chunkWidth;

      var totalChunks = Mathf.CeilToInt((mapGenData.GeneratorParameters.Width + chunkWidth)
         * (mapGenData.GeneratorParameters.Height + chunkWidth) / (float) chunkSize);

      _mapGeometry = GetElement<MapGeometry>();

      Logger.Info("Building fog");

      for (var chunkId = 0; chunkId < totalChunks; chunkId++) {
         var chunkCoords = GetChunkMinMaxCoords(chunkId, maxWidth + chunkWidth, chunkWidth);

         if (cells == null) continue;

         var chunkCells = cells.Where(c => ChunkPositionCondition(c, chunkCoords)).ToArray();
         Logger.Debug($"Chunking: Taking {chunkCells.Length} cells");
         Logger.Debug("---");
         // Remove these cells from the enumeration
         cells = cells.Except(chunkCells).ToArray();

         Logger.Debug($"{cells.Length} cells remaining in map data");
         Logger.Debug($"Building chunk {chunkId}.");

         BuildChunk(chunkId, chunkCoords, chunkCells, chunkWidth);
      }
   }

   private void BuildChunk(int chunkId, Vector3[] chunkCoords, Cell[] chunkCells, int chunkWidth) {
      // Create Fog MultiMesh
      var fogMultiMeshInstance = new MultiMeshInstance {
         Multimesh = new MultiMesh {
            Mesh = _fogMesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
            InstanceCount = chunkWidth * chunkWidth
         },
         CastShadow = GeometryInstance.ShadowCastingSetting.Off,
         PhysicsInterpolationMode = Node.PhysicsInterpolationModeEnum.Off,
         Translation = new Vector3(chunkCoords[0].x, 0, chunkCoords[0].z)
      };
      _mapGeometry?.AddChild(fogMultiMeshInstance);
      fogMultiMeshInstance.Owner = _mapGeometry;

      for (var i = 0; i < fogMultiMeshInstance.Multimesh.InstanceCount; i++) {
         var fogPosition = new Vector3(i % chunkWidth, 0.618f, Mathf.FloorToInt(i / chunkWidth));
         fogMultiMeshInstance.Multimesh.SetInstanceTransform(i, new Transform(Basis.Identity, fogPosition));
      }

      _fogMultiMeshes.Add(fogMultiMeshInstance);
   }

   private bool ChunkPositionCondition(Cell c, IList<Vector3> chunkCoords) {
      return c.Position.x >= chunkCoords[0].x && c.Position.x <= chunkCoords[1].x && c.Position.z >= chunkCoords[0].z
             && c.Position.z <= chunkCoords[1].z;
   }

   public static Vector3[] GetChunkMinMaxCoords(int chunkId, int maxWidth, int chunkWidth) {
      var start = new Vector3(chunkId * chunkWidth % maxWidth, 0, Mathf.FloorToInt(chunkId * (float) chunkWidth / maxWidth) * chunkWidth);

      var end = start + new Vector3(chunkWidth, 0, chunkWidth);
      var coords = new[] {start, end};

      return coords;
   }
}