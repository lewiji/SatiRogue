using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Components;

namespace SatiRogue.Entities; 

public class MeshRendererEntity : Spatial, IGameObject {
   public string Uuid { get; } = Guid.NewGuid().ToString();
   public GameObject? EcOwner { get; set; }
   public Entity? Entity {
      get => EcOwner as Entity;
   }
   public void InitialiseWithParameters(IGameObjectParameters parameters) {
      throw new System.NotImplementedException();
   }
}