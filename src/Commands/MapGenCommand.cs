using SatiRogue.Grid;

namespace SatiRogue.Commands;

public abstract class MapGenCommand : Command {
   public MapGenCommand(MapData mapData) {
      MapData = mapData;
   }

   protected MapData MapData { get; }
}