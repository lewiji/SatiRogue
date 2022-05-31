using System.Collections.Generic;
using Godot;

namespace SatiRogue.Grid.MapGen; 

public class MapGenMapData : AbstractMapData {
   public readonly MapGenParams MapParams;
   public HashSet<Rect2> GeneratorSpaces { get; } = new();
   public MapGenMapData(MapGenParams mapParams) {
      MapParams = mapParams;
   }
}