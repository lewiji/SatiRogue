using System;
using System.Collections;
using System.Linq;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.MathUtils;

namespace SatiRogue.Entities;

public class EntityRegistry : GameObject {
   private static EntityRegistry? _instance;
   private static string? PlayerUuid { get; set; }
   /// <summary>
   ///    Dictionary of Guid strings mapped to Entities
   /// </summary>
   public static System.Collections.Generic.Dictionary<string, Entity> EntityList = new();
   public static EnemyEntity[] EnemyList => (EnemyEntity[]) EntityList.Values.OfType<EnemyEntity>();
   public static PlayerEntity? Player {
      get {
         if (PlayerUuid != null) return (PlayerEntity) EntityList[PlayerUuid];
         return null;
      }
   }

   public static Hashtable BlockedCells = new();
   
   public EntityRegistry() {
      _instance = this;
   }

   public static void RegisterEntity(Entity entity, IGameObjectParameters parameters) {
      if (entity is PlayerEntity playerData)
         PlayerUuid = playerData.Uuid;
      EntityList.Add(entity.Uuid, entity);

      parameters.EcOwner ??= _instance;
      entity.InitialiseWithParameters(parameters);

      if (entity is GridEntity {BlocksCell: true} gridEntity) BlockedCells.Add(gridEntity.GridPosition, entity.Uuid);
      if (entity is StairsEntity stairsEntity) Logger.Info($"Stairs UUID: {stairsEntity.Uuid}");
      
      _instance?.AddChild(entity);
      entity.Owner = _instance;
   }

   public static void UnregisterEntity(Entity entity) {
      EntityList.Remove(entity.Uuid);
      if (entity is PlayerEntity) {
         PlayerUuid = null;
      }
   }

   public static void Clear() {
      BlockedCells.Clear();
      foreach (var keyValuePair in EntityList) {
         if (IsInstanceValid(keyValuePair.Value)) {
            keyValuePair.Value.ClearComponents();
            keyValuePair.Value.QueueFree();
         }
      }
      EntityList.Clear();
      _instance = null;
   }

   public static bool IsPositionBlocked(Vector3i position) {
      return BlockedCells.ContainsKey(position);
   }
}

internal class Uuid { }