using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using SatiRogue.Math;

namespace SatiRogue.Entities;

public class EntityRegistry : Node {
   private static EntityRegistry _instance;
   public static Dictionary<Guid, EntityData> EntityList = new();
   public static Hashtable BlockedCells = new();

   public EntityRegistry() {
      _instance = this;
   }

   public static PlayerData? Player { get; private set; }

   public static void RegisterEntity(EntityData entityData) {
      if (entityData is PlayerData playerData)
         Player = playerData;
      else
         EntityList.Add(entityData.Uuid, entityData);

      if (entityData.BlocksCell) BlockedCells.Add(entityData.GridPosition, entityData.Uuid);

      _instance.AddChild(entityData);
   }

   public static bool IsPositionBlocked(Vector3i position) {
      return BlockedCells.ContainsKey(position);
   }
}