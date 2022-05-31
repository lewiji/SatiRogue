using System;
using System.Linq;
using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Behaviours;
using SatiRogue.Entities;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;

namespace SatiRogue.Commands.MapGen;

public class MapGenPlaceEnemies : MapGenCommand {
   public MapGenPlaceEnemies(MapGenMapData mapData) : base(mapData) { }

   public override Error Execute() {
      for (var i = 0; i < MapData.MapParams.NumEnemies; i++) {
         var startingRoom = MapData.GeneratorSpaces.ElementAt((int) GD.RandRange(0, MapData.GeneratorSpaces.Count));
         var startX = (int) startingRoom.Position.x + (int) GD.RandRange(0, (int) startingRoom.Size.x);
         var startY = (int) startingRoom.Position.y + (int) GD.RandRange(0, (int) startingRoom.Size.y);
         var startVec = new Vector3i(startX, 0, startY);

         if (EntityRegistry.IsPositionBlocked(startVec)) continue;

         var enemyTypes = Enum.GetValues(typeof(EntityTypes));
         var enemyType = (EntityTypes) enemyTypes.GetValue(GD.Randi() % enemyTypes.Length);

         EntityRegistry.RegisterEntity(
            new EnemyEntity(),
            new EnemyEntityParameters {
               GridPosition = startVec,
               EntityType = enemyType,
               BlocksCell = true,
               Name = enemyType.ToString(),
               Visible = false,
               Components = new Component[] {new EnemyBehaviourTreeComponent()}
            }
         );
      }

      return Error.Ok;
   }
}