using System.Collections.Generic;
using SatiRogue.Ecs.Dungeon.Components.Actor;

namespace SatiRogue.Resources;

public static class EnemyData
{
    public enum Species
    {
        Harpy,
        Maw,
        Rat,
        FireKasina
    }
    
    public enum Level {
        LowlyEnemy
    }
    
    public static readonly Godot.Collections.Dictionary<Species, string> HumanReadableNames = new()
    {
        { Species.Harpy, "Harpy" },
        { Species.Maw, "Maw" },
        { Species.Rat, "Rat" },
        { Species.FireKasina, "FireKasina" }
    };

    public static readonly Dictionary<Level, Stats.StatsRecordClass> InitialStats = new() {
        {
            Level.LowlyEnemy,
            new Stats.StatsRecordClass {
                Health = 1,
                SightRange = 8,
                Speed = 1,
                Strength = 1,
                Defence = 0
            }
        }
    };

    public const Level DefaultEnemyLevel = Level.LowlyEnemy;
}