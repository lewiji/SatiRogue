using Godot;
namespace SatiRogue.Ecs.Play.Triggers;

public class VisibilityChangedTrigger {
   public Vector3[] Positions;
   public VisibilityChangedTrigger(Vector3[] positions) { Positions = positions; }
}