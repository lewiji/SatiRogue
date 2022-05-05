using Godot;
using RoguelikeMono.Ecs.Systems;

namespace RoguelikeMono.Ecs.Components; 

public class CMeshRenderer : Component {
   public CMeshRenderer(Mesh mesh) {
      MeshSystem.Register(this);
      var meshInstance = new MeshInstance();
      meshInstance.Mesh = mesh;
   }
}