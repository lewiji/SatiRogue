using Godot;
using RoguelikeMono.Ecs.Components;

namespace RoguelikeMono.Ecs.Entities; 

public class ESpatialMesh : Entity {
   public ESpatialMesh(Vector3 position) {
      CTransform transform = new CTransform();
      transform.Position = position;
      AddComponent(transform);

      CubeMesh mesh = new CubeMesh();
      mesh.Size = new Vector3(1f, 1f, 1f);
      CMeshRenderer meshRenderer = new CMeshRenderer(mesh);
      AddComponent(meshRenderer);
   }
}