using Godot;
using RoguelikeMono.Ecs.Components;

namespace RoguelikeMono.Ecs.Entities;

public class ESpatialMesh : Entity {
   public ESpatialMesh(Vector3 position) {
      var transform = new CTransform();
      transform.Position = position;
      AddComponent(transform);

      var mesh = new CubeMesh();
      mesh.Size = new Vector3(1f, 1f, 1f);
      var meshRenderer = new CMeshRenderer(mesh);
      AddComponent(meshRenderer);
   }
}