using Godot;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
namespace SatiRogue.Ecs.Core.Systems;

public class InitFade : GdSystem {
   private static readonly PackedScene FadeScene = GD.Load<PackedScene>("res://src/Ecs/Core/Nodes/Fade.tscn");

   public override void Run() {
      var fade = FadeScene.Instance<Fade>();
      World.GetElement<CoreState>().AddChild(fade);
      World.AddElement(fade);
   }
}