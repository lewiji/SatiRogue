using Godot;

namespace SatiRogue.Ecs.Play.Nodes.Actors; 

public class Character : Spatial {
   [Export] protected int Health = 10;
   [Export] protected int Strength = 1;
   [Export] protected float Speed = 1;
   [Export] public bool BlocksCell = true;
}