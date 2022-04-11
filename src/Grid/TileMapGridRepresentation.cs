using Godot;

namespace RoguelikeMono.Grid;

public class TileMapGridRepresentation : TileMap
{
    [Export] private NodePath? _gridGeneratorNodePath { get; set; }
    private GridGenerator? _gridGenerator;

    public override void _Ready()
    {
        if (_gridGeneratorNodePath != null) _gridGenerator = GetNode<GridGenerator>(_gridGeneratorNodePath);
        if (_gridGenerator != null)
        {
            _gridGenerator.Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
        }
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
            CellType.Floor => 8,
            CellType.Stairs => 2,
            CellType.Wall => 7,
            CellType.DoorClosed => 5,
            CellType.DoorOpen => 0,
            _ => 0
        };
    }
}