using System.Collections.Generic;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Play.Components.Actor;

public class Stats {
   public StatsRecordClass Record;

   public record StatsRecordClass {
      public int Health { get; set; } = default!;
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

   public static readonly Dictionary<EnemyClass, StatsRecordClass> EnemyClassInitialStats = new() {
      {
         EnemyClass.LowlyEnemy,
         new StatsRecordClass {
            Health = 1,
            SightRange = 8,
            Speed = 1,
            Strength = 1,
            Defence = 0
         }
      }
   };

   public const PlayerClass DefaultPlayerClass = PlayerClass.Worldling;
   public const EnemyClass DefaultEnemyClass = EnemyClass.LowlyEnemy;

   public Stats() {
      Record = EnemyClassInitialStats[DefaultEnemyClass];
      Logger.Warn($"Stats: No EnemyClass/PlayerClass passed into Stats constructor. Constructing default {DefaultEnemyClass}.");
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

   public Stats(EnemyClass characterClass) {
      if (EnemyClassInitialStats.ContainsKey(characterClass)) {
         Record = EnemyClassInitialStats[characterClass];
      } else {
         Record = EnemyClassInitialStats[DefaultEnemyClass];

         Logger.Warn(
            $"Stats: Couldn't find stats record for enemy class: {characterClass}. Constructing default {DefaultEnemyClass} instead.");
      }
   }
}