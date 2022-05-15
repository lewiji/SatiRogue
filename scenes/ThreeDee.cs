using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Grid;

namespace SatiRogue.scenes;

public partial class ThreeDee : Spatial {
   [OnReadyGet("Enemies", Export = true)] public Spatial? EnemiesSpatial;

   [OnReadyGet("../../../../GridGenerator", Export = true)]
   public MapGenerator? GridGenerator;
}