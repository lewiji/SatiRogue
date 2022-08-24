using Godot;
using RelEcs;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Loading.Systems; 

public class PreloadResources : GdSystem {
   private static readonly PackedScene ResourcePreloaderScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ResourcePreloader.tscn");

   public override void Run() {
      var preloader = ResourcePreloaderScene.Instance<ResourcePreloader>();
      GetElement<LoadingState>().AddChild(preloader);
      AddElement(preloader);
      Logger.Info($"Preloading {preloader.GetResourceList().Length} resources.");
   }
}