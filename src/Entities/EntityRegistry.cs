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

   public override void _EnterTree() {
      _instance = this;
   }

   public override void _ExitTree() {
      Clear();
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
      if (entity is PlayerEntity) {
         PlayerUuid = null;
      }
      EntityList.Remove(entity.Uuid);
   }

   public static void Clear() {
      BlockedCells.Clear();
      var arr = EntityList.ToArray();
      foreach (var keyValuePair in arr.Where(keyValuePair => IsInstanceValid(keyValuePair.Value))) {
         keyValuePair.Value.ClearComponents();
         UnregisterEntity(keyValuePair.Value);
         keyValuePair.Value.QueueFree();
      }
      _instance = null;
   }

   private static void RemoveUuidFromList(string id) {
      
   }

   public static bool IsPositionBlocked(Vector3i position) {
      return BlockedCells.ContainsKey(position);
   }
}

internal class Uuid { }