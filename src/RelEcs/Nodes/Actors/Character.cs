using Godot;

namespace SatiRogue.RelEcs.Nodes.Actors; 

public class Character : Spatial {
   [Export] protected int Health = 10;
   [Export] protected int Strength = 1;
   [Export] protected float Speed = 1;
}