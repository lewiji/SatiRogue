using SatiRogue.Commands;
using SatiRogue.Commands.MapGen;

namespace SatiRogue.Grid.MapGen.Strategies; 

public class RoomsAndCorridors : MapGenStrategy {
   public RoomsAndCorridors(MapGenParams mapGenParams) : base(mapGenParams) {
      CommandQueue.Add(new MapGenCreateRooms(MapData));
      CommandQueue.Add(new MapGenCreateCorridors(MapData));
      CommandQueue.Add(new MapGenFloodFill(MapData));
      CommandQueue.Add(new MapGenPlacePlayer(MapData));
      CommandQueue.Add(new MapGenPlaceEnemies(MapData));
   }
}