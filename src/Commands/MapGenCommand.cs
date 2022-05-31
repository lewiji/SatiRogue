using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;

namespace SatiRogue.Commands;

public abstract class MapGenCommand : Command {
   public MapGenCommand(MapGenMapData mapData) {
      MapData = mapData;
   }

   protected MapGenMapData MapData { get; }
}