using Godot;
using SatiRogue.Ecs.Core.Nodes;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Core.Systems;

public partial class InitFade : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene FadeScene = GD.Load<PackedScene>("res://src/Ecs/Core/Nodes/Fade.tscn");

   public void Run() {
      var fade = FadeScene.Instance<Fade>();
      World.GetElement<CoreState>().AddChild(fade);
      World.AddElement(fade);
   }
}