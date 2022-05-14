using System;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Grid.Entities;
using SatiRogue.Math;
using SatiRogue.scenes;

namespace SatiRogue.Grid;

public class TileMapGridRepresentation : TileMap {
    public static int? TileSize;
    private Camera2D? _camera2D;
    private Label? _debugLabel;
    private Node2D? _enemiesNode2D;
    [Export] private NodePath? _cameraNodePath { get; set; }
    [Export] private NodePath? _twoDeeNodePath { get; set; }
    [Export] private NodePath? _enemiesNodePath { get; set; }

    public override void _Ready() {
        Clear();

        TileSize = (int) CellSize.x;

        _debugLabel = GetNode<Label>("Label");
        _camera2D = GetNode<Camera2D>(_cameraNodePath);
        _enemiesNode2D = GetNode<Node2D>(_enemiesNodePath);

        CallDeferred(nameof(ConnectToGridGenerator));
    }

    private void ConnectToGridGenerator() {
        var gridGenerator = GetNode<TwoDee>(_twoDeeNodePath).GridGenerator;
        gridGenerator?.Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
        gridGenerator?.Connect(nameof(GridGenerator.VisibilityChanged), this, nameof(OnVisibilityChanged));
    }

    private void OnMapDataChanged() {
        Logger.Info("2d: Mapdata changed");
        var cells = GridGenerator._mapData.Cells;
        foreach (var cell in cells) {
            if (GetTileId(cell) is { } cellValue) {
                SetCell(cell.Position.x, cell.Position.z, cellValue);
            }
        }
        
        foreach (var enemyData in EntityRegistry.EnemyList) {
            switch (enemyData.EnemyType) {
                case EnemyTypes.Maw:
                    var tilePosition = enemyData.GridPosition * TileMapGridRepresentation.TileSize;
                    var enemyNode = new Node2D {
                        Position = new Vector2(tilePosition.GetValueOrDefault().x, tilePosition.GetValueOrDefault().z)
                    };
                    var sprite = new AnimatedSprite {
                        Frames = GD.Load<SpriteFrames>("res://scenes/ThreeDee/res/enemy/maw/maw_purple_sprite_Frames.tres"),
                        Material = GD.Load<ShaderMaterial>("res://scenes/TwoDee/shader/canvas_outline_material.tres"),
                        Playing = true,
                        Animation = "idle",
                        Centered = true,
                        Scale = new Vector2(0.5f, 0.5f),
                        Position = new Vector2(4f, 2f),
                        ZIndex = 1
                    };
                    enemyNode.AddChild(sprite);
                    _enemiesNode2D?.AddChild(enemyNode);
                    break;
                case EnemyTypes.Ratfolk:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void OnVisibilityChanged(Vector3[] positions) {
        Logger.Info("Tilemap visibility updating");
        var cells = GridGenerator._mapData.Cells;
        foreach (var cell in cells) {
            if (GetTileId(cell) is { } cellValue) {
                SetCell(cell.Position.x, cell.Position.z, cellValue);
            }
        }
    }

    private static int? GetTileId(Cell cell) {
        if (cell.Visibility == CellVisibility.Unseen) return null;
        return cell.Type switch {
            CellType.Floor => 12,
            CellType.Stairs => 2,
            CellType.Wall => 17,
            CellType.DoorClosed => 5,
            CellType.DoorOpen => 0,
            _ => 0
        };
    }

    public override void _Input(InputEvent @event) {
        if (_debugLabel == null || _camera2D == null) return;
        if (@event is InputEventMouse inputEventMouse)
            _debugLabel.Text = (inputEventMouse.Position / CellSize * _camera2D.Zoom).Round().ToString();
    }
}