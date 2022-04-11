using Godot;
using RoguelikeMono.scenes;

namespace RoguelikeMono.Grid;

public class TileMapGridRepresentation : TileMap
{
   
    private Label? _debugLabel;
    [Export] private NodePath? _cameraNodePath { get; set; }
    private Camera2D? _camera2D;
    

    public static int? TileSize;

    public override void _Ready()
    {
        Clear();
        
        TileSize = (int)CellSize.x;

        _debugLabel = GetNode<Label>("Label");
        _camera2D = GetNode<Camera2D>(_cameraNodePath);
        
        CallDeferred(nameof(ConnectToGridGenerator));
    }

    private void ConnectToGridGenerator() {
        var gridGenerator = (GetParent() as TwoDee)?.GridGenerator;
        gridGenerator?.Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
    }

    private void OnMapDataChanged()
    {
        GD.Print("Mapdata changed");
        var cells = GridGenerator._mapData.Cells;
        foreach (var cell in cells)
        {
            SetCell(cell.Position.x, cell.Position.z, GetTileId(cell));
        }
    }

    private int GetTileId(Cell cell)
    {
        return cell.CellType switch
        {
            CellType.Floor => 12,
            CellType.Stairs => 2,
            CellType.Wall => 17,
            CellType.DoorClosed => 5,
            CellType.DoorOpen => 0,
            _ => 0
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (_debugLabel == null || _camera2D == null) return;
        if (@event is InputEventMouse inputEventMouse)
        {
            _debugLabel.Text = ((inputEventMouse.Position / CellSize) * _camera2D.Zoom).Round().ToString();
        }
    }
}