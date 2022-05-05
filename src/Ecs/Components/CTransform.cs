using Godot;
using RoguelikeMono.Ecs.Systems;

namespace RoguelikeMono.Ecs.Components; 

public class CTransform : Component {
   public CTransform() {
      TransformSystem.Register(this);
   }
   
   public Vector3 Position = Vector3.Zero;
   public Vector3 Scale = Vector3.Zero;
   public Basis Basis = Basis.Identity;
}