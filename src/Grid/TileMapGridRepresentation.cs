using System.Linq;
using Godot;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using GoDotNet;
using SatiRogue.MathUtils;
using SatiRogue.scenes;
using Enemy2D = SatiRogue.Entities.VisualRepresentation.Enemy2D;

namespace SatiRogue.Grid;

public class TileMapGridRepresentation : TileMap, IDependent {
   public static int? TileSize;
   private Camera2D? _camera2D;
   private Label? _debugLabel;
   private Node2D? _enemiesNode2D;
   [Export] private NodePath? _cameraNodePath { get; set; }
   [Export] private NodePath? _twoDeeNodePath { get; set; }
   [Export] private NodePath? _enemiesNodePath { get; set; }
   [Dependency] private RuntimeMapNode RuntimeMapNode => this.DependOn<RuntimeMapNode>();

   public override void _Ready() {
      Clear();

      TileSize = (int) CellSize.x;

      _debugLabel = GetNode<Label>("Label");
      _debugLabel.SetAsToplevel(true);
      _camera2D = GetNode<Camera2D>(_cameraNodePath);
      _enemiesNode2D = GetNode<Node2D>(_enemiesNodePath);

      this.Depend();
   }

   public void Loaded() {
      RuntimeMapNode.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(OnMapDataChanged));
      RuntimeMapNode.Connect(nameof(RuntimeMapNode.VisibilityChanged), this, nameof(OnVisibilityChanged));
   }

   private async void OnMapDataChanged() {
      await ToSignal(GetTree(), "idle_frame");
      var cells = RuntimeMapNode.MapData?.Cells;
      if (cells != null)
         foreach (var cell in cells)
            if (GetTileId(cell) is { } cellValue)
               SetCell(cell.Position.x, cell.Position.z, cellValue);

      foreach (var entityData in EntityRegistry.EntityList)
         if (entityData.Value is EnemyEntity enemyData) {
            var tilePosition = enemyData.GridPosition * TileSize;
            var enemyNode = new Enemy2D(enemyData) {
               Position = new Vector2(tilePosition.GetValueOrDefault().x, tilePosition.GetValueOrDefault().z)
            };
            var sprite = new AnimatedSprite {
               Frames = EntityResourceLocator.GetResource<SpriteFrames>(enemyData.EntityType),
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
         }
   }

   private void Redraw() {
      var cells = RuntimeMapNode.MapData?.GetSurroundingCells(EntityRegistry.Player!.GridPosition).GetEnumerator();
      if (cells == null) return;
      var startVec = new Vector3i(0, 0, 0);
      var endVec = new Vector3i(MapData.SurroundingCellsRadius, 0, MapData.SurroundingCellsRadius);
      
      for (var y = startVec.z; y < endVec.z; y++) {
         for (var x = startVec.x; x < endVec.x; x++) {
            cells.MoveNext();
            var cell = cells.Current;
            if (cell != null && GetTileId(cell) is { } cellValue)
               SetCell(cell.Position.x, cell.Position.z, cellValue);

         }
      }
   }

   private void OnVisibilityChanged(Vector3[] positions) {
      Redraw();
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