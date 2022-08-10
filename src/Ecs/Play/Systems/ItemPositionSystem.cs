using RelEcs;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Items;
namespace SatiRogue.Ecs.Play.Systems;

public class ItemPositionSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var query = QueryBuilder<Item, GridPositionComponent>().Build();

      foreach (var (item, gridPos) in query) { }
   }
}