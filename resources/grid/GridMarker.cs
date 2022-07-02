using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Grid;
using SatiRogue.MathUtils;
#pragma warning disable CS0649

#pragma warning disable CS8618

namespace SatiRogue.resources.grid; 

public partial class GridMarker : MeshInstance {
    [OnReadyGet("DebugInfo/LabelCoords")] private Label3D _labelCoords;
    [OnReadyGet("DebugInfo/LabelCellBlocked")] private Label3D _labelBlocked;

    public void Move(Vector3 to) {
        Translation = to.Snapped(new Vector3(1f, 0, 1f)) + new Vector3(0, -0.26f, 0);
        var currentCell = RuntimeMapNode.Instance?.MapData?.GetCellAt(new Vector3i(Translation));
        if (currentCell != null) {
            _labelCoords.Text = $"{currentCell.Position.x},{currentCell.Position.z}";
            _labelBlocked.Text = currentCell.Blocked ? "Blocked" : "";
        }
    }
}