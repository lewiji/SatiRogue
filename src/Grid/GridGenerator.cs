using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SatiRogue.Grid.Entities;
using SatiRogue.Math;
using SatiRogue.Tools;

namespace SatiRogue.Grid;

public class GridGenerator : Node {
    [Signal]
    public delegate void MapChanged();

    public static MapData _mapData = new();
    private static readonly int _height = 512;
    private static readonly int _roomMaxWidth = 108;
    private static readonly int _roomMinWidth = 3;
    private static readonly int _rooms = 102;
    private static readonly int _width = 512;

    public static Godot.Collections.Dictionary<string, int> GetParams() => new() {{"Height", _height}, {"MaxWidth", _roomMaxWidth}, 
        {"MinWidth", _roomMinWidth}, {"Rooms", _rooms}, {"Width", _width}}; 

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
        PlacePlayer(voidSpaces);


        GD.Print("Finished generating grid.");
        CallDeferred(nameof(EmitMapChangedSignal));
    }

    private void EmitMapChangedSignal() {
        EmitSignal(nameof(MapChanged));
    }

    private void PlacePlayer(HashSet<Rect2> voidSpaces) {
        if (EntityRegistry.Player == null) throw new Exception("Trying to place player, but player is not initialised.");
        var startingRoom = voidSpaces.ElementAt(Rng.IntRange(0, voidSpaces.Count));
        var startX = (int) startingRoom.Position.x + Rng.IntRange(0, (int) startingRoom.Size.x);
        var startY = (int) startingRoom.Position.y + Rng.IntRange(0, (int) startingRoom.Size.y);
        EntityRegistry.Player.GridPosition = new Vector3i(startX, 0, startY);

        GD.Print($"Moved player to {EntityRegistry.Player.GridPosition}");
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
            switch (cell.CellType) {
                // null is an uncarved space, make it a wall
                case null:
                    cell.SetCellType(CellType.Wall);
                    break;
                case CellType.Void:
                    // set Void, non-null spaces we've traversed to Floors
                    cell.CellType = CellType.Floor;

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
            var roomMaxX = _width - roomWidth;
            var roomMaxY = _height - roomHeight;
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