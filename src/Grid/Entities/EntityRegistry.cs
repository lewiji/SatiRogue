using Godot;

namespace SatiRogue.Grid.Entities;

public class EntityRegistry : Node {
    public static PlayerData? Player { get; private set; }

    public override void _Ready() {
        Player = GetNode<PlayerData>("PlayerData");
    }
}