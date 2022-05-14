using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Math;
using SatiRogue.Tools;
using EnemyData = SatiRogue.Entities.EnemyData;

namespace SatiRogue.Grid;

public class MapGenerator : Node {
    [Signal] public delegate void MapChanged();
    [Signal] public delegate void VisibilityChanged(Vector3[] changedCells);

    public AStar AStar = new ();
    public static NodePath? Path;
    public static readonly int Height = 150;
    private static readonly int _roomMaxWidth = 32;
    private static readonly int _roomMinWidth = 3;
    private static readonly int _rooms = 35;
    public static readonly int Width = 150;
    public static MapData _mapData = new(Width, Height);

    public static Godot.Collections.Dictionary<string, int> GetParams() => new() {{"Height", Height}, {"MaxWidth", _roomMaxWidth}, 
        {"MinWidth", _roomMinWidth}, {"Rooms", _rooms}, {"Width", Width}};

    public override void _EnterTree() {
        Path = GetPath();
    }

    public override void _Ready() {
        GD.Randomize();
        CallDeferred(nameof(StartGeneration));
    }

    private void StartGeneration() {
        // Carve void tiles as potential rooms
        var voidSpaces = AddFloorSpace();
        // Connect spaces together
        AddCorridors(voidSpaces);
        // Flood fill spaces -> floor + walls
        FloodFillVoidSpaces(voidSpaces);
        _mapData.ConnectPathfindingPoints();
        PlacePlayer(voidSpaces);
        PlaceEnemies(voidSpaces);

        Logger.Info("Finished generating grid.");
        CallDeferred(nameof(EmitMapChangedSignal));
    }

    public override void _Process(float delta) {
        if (_mapData.CellsVisibilityChanged.Count > 0) {
            EmitSignal(nameof(VisibilityChanged), _mapData.CellsVisibilityChanged.ToArray());
            _mapData.CellsVisibilityChanged.Clear();
        }
    }

    private void EmitMapChangedSignal() {
        EmitSignal(nameof(MapChanged));
    }

    private void PlacePlayer(HashSet<Rect2> voidSpaces) {
        var startingRoom = voidSpaces.ElementAt(Rng.IntRange(0, voidSpaces.Count));
        var startX = (int) startingRoom.Position.x + Rng.IntRange(0, (int) startingRoom.Size.x);
        var startY = (int) startingRoom.Position.y + Rng.IntRange(0, (int) startingRoom.Size.y);
        EntityRegistry.RegisterEntity( new PlayerData(new Vector3i(startX, 0, startY)));

        Logger.Info($"Created player at {EntityRegistry.Player?.GridPosition}");
    }
    
    private void PlaceEnemies(HashSet<Rect2> voidSpaces) {
        for (var i = 0; i < 100; i++) {
            
            var startingRoom = voidSpaces.ElementAt(Rng.IntRange(0, voidSpaces.Count));
            var startX = (int) startingRoom.Position.x + Rng.IntRange(0, (int) startingRoom.Size.x);
            var startY = (int) startingRoom.Position.y + Rng.IntRange(0, (int) startingRoom.Size.y);
            var startVec = new Vector3i(startX, 0, startY);
            if (!EntityRegistry.IsPositionBlocked(startVec)) {
                EntityRegistry.RegisterEntity(new EnemyData(startVec, EnemyTypes.Maw));
            }
        }
    } 

    private void AddCorridors(HashSet<Rect2> voidSpaces) {
        var arr = voidSpaces.ToArray();
        for (var i = 1; i < arr.Length; i++) {
            var lastSpace = arr[i - 1];
            var currSpace = arr[i];
            var lastCentre = lastSpace.Position + lastSpace.Size / 2f;
            var currCentre = currSpace.Position + currSpace.Size / 2f;

            if (lastCentre.x < currCentre.x)
                for (var x = (int) lastCentre.x; x < (int) currCentre.x; x++)
                    _mapData.SetCellType(new Vector3i(x, 0, (int) lastCentre.y), CellType.Void);
            else
                for (var x = (int) lastCentre.x; x > (int) currCentre.x; x--)
                    _mapData.SetCellType(new Vector3i(x, 0, (int) lastCentre.y), CellType.Void);

            if (lastCentre.y < currCentre.y)
                for (var y = (int) lastCentre.y; y < (int) currCentre.y; y++)
                    _mapData.SetCellType(new Vector3i((int) currCentre.x, 0, y), CellType.Void);
            else
                for (var y = (int) lastCentre.y; y > (int) currCentre.y; y--)
                    _mapData.SetCellType(new Vector3i((int) currCentre.x, 0, y), CellType.Void);
        }
    }

    private void FloodFillVoidSpaces(HashSet<Rect2> voidSpaces) {
        var startPoint = voidSpaces.First();
        var centre = startPoint.Position + startPoint.Size / 2f;
        FloodFillWalls((int) centre.x, (int) centre.y);
    }

    /** Flood fill from centre of voids, convert voids to floor, and set edges (nulls) as walls **/
    private void FloodFillWalls(int posX, int posY) {
        var tiles = new Stack<Vector3i>();
        tiles.Push(new Vector3i(posX, 0, posY));

        while (tiles.Count > 0) {
            var position = tiles.Pop();
            var cell = _mapData.GetCellAt(position);
            switch (cell.Type) {
                // null is an uncarved space, make it a wall
                case null:
                    cell.SetCellType(CellType.Wall);
                    break;
                case CellType.Void:
                    // set Void, non-null spaces we've traversed to Floors
                    cell.Type = CellType.Floor;

                    // Flood fill recursively
                    tiles.Push(new Vector3i(position.x - 1, 0, position.z));
                    tiles.Push(new Vector3i(position.x + 1, 0, position.z));
                    tiles.Push(new Vector3i(position.x, 0, position.z - 1));
                    tiles.Push(new Vector3i(position.x, 0, position.z + 1));
                    tiles.Push(new Vector3i(position.x + 1, 0, position.z + 1));
                    tiles.Push(new Vector3i(position.x - 1, 0, position.z + 1));
                    tiles.Push(new Vector3i(position.x + 1, 0, position.z - 1));
                    tiles.Push(new Vector3i(position.x - 1, 0, position.z - 1));
                    break;
            }
        }
    }

    private HashSet<Rect2> AddFloorSpace() {
        // Carve void spaces
        var floorSpaces = new HashSet<Rect2>();
        for (var roomIndex = 0; roomIndex < _rooms; roomIndex++) {
            var roomWidth = Rng.IntRange(_roomMinWidth, _roomMaxWidth);
            var roomHeight = Rng.IntRange(_roomMinWidth, _roomMaxWidth);
            var roomMaxX = Width - roomWidth;
            var roomMaxY = Height - roomHeight;
            var roomX = Rng.IntRange(1, roomMaxX);
            var roomY = Rng.IntRange(1, roomMaxY);
            var floorspace = new Rect2(roomX, roomY, roomWidth, roomHeight);
            CarveFloorSpace(floorspace);
            floorSpaces.Add(floorspace);
        }

        return floorSpaces;
    }

    /** Initialise floor space by carving out Void cells **/
    private void CarveFloorSpace(Rect2 rect) {
        for (var x = Mathf.RoundToInt(rect.Position.x); x < Mathf.RoundToInt(rect.End.x); x++)
        for (var z = Mathf.RoundToInt(rect.Position.y); z < Mathf.RoundToInt(rect.End.y); z++) {
            var position = new Vector3i(x, 0, z);
            var cell = _mapData.SetCellType(position, CellType.Void);
        }
    }

    private CellType GetCellTypeForRoom(Rect2 rect, int x, int y) {
        if (x == Mathf.RoundToInt(rect.Position.x) || y == Mathf.RoundToInt(rect.Position.y) ||
            x == Mathf.RoundToInt(rect.End.x) - 1 || y == Mathf.RoundToInt(rect.End.y) - 1)
            return CellType.Wall;

        return CellType.Floor;
    }
}