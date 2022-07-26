using Godot;

namespace SatiRogue.Ecs.Play.Nodes.Actors; 

public class Character : Spatial {
   [Export] public int Health = 10;
   [Export] public int Strength = 1;
   [Export] public float Speed = 1;
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;
   [Export] public bool Behaving => Health > 0 && Enabled;
}