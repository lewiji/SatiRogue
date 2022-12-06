using System.Collections.Generic;
using SatiRogue.Debug;
using SatiRogue.Resources;

namespace SatiRogue.Ecs.Dungeon.Components.Actor;

public partial class Stats {
   public StatsRecordClass Record;

   public record StatsRecordClass {
      public int Health { get; set; }
      public int SightRange { get; set; }
      public int Speed { get; set; }
      public int Strength { get; set; }
      public int Defence { get; set; }
   }

   public static readonly Dictionary<PlayerClass, StatsRecordClass> PlayerClassInitialStats = new() {
      {
         PlayerClass.Worldling,
         new StatsRecordClass {
            Health = 10,
            SightRange = 10,
            Speed = 1,
            Strength = 1,
            Defence = 0
         }
      }
   };


   public const PlayerClass DefaultPlayerClass = PlayerClass.Worldling;

   public Stats() {
      Record = EnemyData.InitialStats[EnemyData.DefaultEnemyLevel];
      Logger.Warn($"Stats: No EnemyClass/PlayerClass passed into Stats constructor. Constructing default {EnemyData.DefaultEnemyLevel}.");
   }

   public Stats(PlayerClass characterClass) {
      if (PlayerClassInitialStats.ContainsKey(characterClass)) {
         Record = PlayerClassInitialStats[characterClass];
      } else {
         Record = PlayerClassInitialStats[DefaultPlayerClass];

         Logger.Warn(
            $"Stats: Couldn't find stats record for player class: {characterClass}. Constructing default {DefaultPlayerClass} instead.");
      }
   }

   public Stats(EnemyData.Level characterClass) {
      if (EnemyData.InitialStats.ContainsKey(characterClass)) {
         Record = EnemyData.InitialStats[characterClass];
      } else {
         Record = EnemyData.InitialStats[EnemyData.DefaultEnemyLevel];

         Logger.Warn(
            $"Stats: Couldn't find stats record for enemy class: {characterClass}. Constructing default {EnemyData.DefaultEnemyLevel} instead.");
      }
   }
}