using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Dungeon.Nodes; 

public partial class DungeonDirectionalLight : Spatial
{
   [OnReadyGet("GIProbe")] GIProbe _giProbe { get; set; }

   [OnReady] public void InitialBake() {
      CallDeferred(nameof(BakeLighting));
   }
   
   public void BakeLighting() {
      Logger.Info("Baking lights...");
      _giProbe.Bake();
      Logger.Info("Baked!");
   }
}