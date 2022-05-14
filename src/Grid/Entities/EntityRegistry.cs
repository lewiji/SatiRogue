using System.Collections.Generic;
using Godot;
using SatiRogue.Math;

namespace SatiRogue.Grid.Entities;

public class EntityRegistry : Node {
    public static PlayerData? Player { get; private set; }
    public static List<EnemyData> EnemyList = new();

    public override void _Ready() {
        Player = GetNode<PlayerData>("PlayerData");
    }

    public static void RegisterEnemy(EnemyData enemyData) {
        EnemyList.Add(enemyData);
    }
}