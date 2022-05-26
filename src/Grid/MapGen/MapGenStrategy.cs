using SatiRogue.Commands;

namespace SatiRogue.Grid.MapGen; 

public abstract class MapGenStrategy : IMapGenStrategy {
   protected CommandQueue CommandQueue = new();
   protected MapData MapData;
   public MapGenStrategy(MapGenParams mapGenParams) {
      MapData = new(mapGenParams);
   }

   public virtual MapData GenerateMap() {
      CommandQueue.ExecuteAll();
      return MapData;
   }
}