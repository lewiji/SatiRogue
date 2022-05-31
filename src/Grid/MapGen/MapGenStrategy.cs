using SatiRogue.Commands;

namespace SatiRogue.Grid.MapGen; 

public abstract class MapGenStrategy : IMapGenStrategy {
   protected CommandQueue CommandQueue = new();
   protected MapGenMapData MapData;
   public MapGenStrategy(MapGenParams mapGenParams) {
      MapData = new(mapGenParams);
   }

   public virtual MapGenMapData GenerateMap() {
      CommandQueue.ExecuteAll();
      return MapData;
   }
}