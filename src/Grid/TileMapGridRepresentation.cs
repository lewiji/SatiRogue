using Godot;
using SatiRogue.Debug;
using SatiRogue.Math;
using SatiRogue.scenes;

namespace SatiRogue.Grid;

public class TileMapGridRepresentation : TileMap {
    public static int? TileSize;
    private Camera2D? _camera2D;
    private Label? _debugLabel;
    [Export] private NodePath? _cameraNodePath { get; set; }

    [Export] private NodePath? _twoDeeNodePath { get; set; }

    public override void _Ready() {
        Clear();

        TileSize = (int) CellSize.x;

        _debugLabel = GetNode<Label>("Label");
        _camera2D = GetNode<Camera2D>(_cameraNodePath);

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