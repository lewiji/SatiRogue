using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Grid;

namespace SatiRogue.scenes;

public partial class ThreeDee : Spatial {
    [OnReadyGet("../../../../GridGenerator", Export = true)]
    public GridGenerator? GridGenerator;

    [OnReadyGet("Enemies", Export = true)] public Spatial? EnemiesSpatial;
}