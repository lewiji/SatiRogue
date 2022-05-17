using System;
using Godot;
using SatiRogue.Commands;
using SatiRogue.Entities;

namespace SatiRogue.Components; 

public abstract class Component : Entity, IComponent {
   public override GameObject? Parent => base.Parent as Entity;
   

   public override void _EnterTree() {
      base._EnterTree();
      Name = "Component";
   }
}