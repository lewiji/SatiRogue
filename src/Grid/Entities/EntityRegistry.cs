using Godot;

namespace RoguelikeMono.Grid.Entities;

public class EntityRegistry : Node
{
    public static PlayerData? Player
    {
        get => _player;
    }
    private static PlayerData? _player;
    public override void _Ready()
    {
        _player = GetNode<PlayerData>("PlayerData");
    }
}