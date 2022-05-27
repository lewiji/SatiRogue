using System.Collections;
using System.Collections.Generic;
using SatiRogue.MathUtils;

namespace SatiRogue.Entities;

public class EntityRegistry : GameObject {
   private static EntityRegistry? _instance;

   /// <summary>
   ///    Dictionary of Guid strings mapped to Entities
   /// </summary>
   public static Dictionary<string, Entity> EntityList = new();

   public static Hashtable BlockedCells = new();

   public EntityRegistry() {
      _instance = this;
   }

   public static PlayerEntity? Player { get; private set; }

   public static void RegisterEntity(Entity entity, IGameObjectParameters parameters) {
      if (entity is PlayerEntity playerData)
         Player = playerData;
      else
         EntityList.Add(entity.Uuid, entity);

      parameters.Parent ??= _instance;
      entity.InitialiseWithParameters(parameters);

      if (entity is GridEntity {BlocksCell: true} gridEntity) BlockedCells.Add(gridEntity.GridPosition, entity.Uuid);

      _instance?.AddChild(entity);
   }

   public static bool IsPositionBlocked(Vector3i position) {
      return BlockedCells.ContainsKey(position);
   }
}