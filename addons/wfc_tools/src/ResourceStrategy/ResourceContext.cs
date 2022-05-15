using System;
using Godot;

namespace SatiRogue.addons.wfc_tools.src.ResourceStrategy;

public class ResourceContext : Node {
   private IResourceStrategy? _resourceStrategy;
   public ResourceContext() { }

   public ResourceContext(IResourceStrategy strategy) {
      _resourceStrategy = strategy;
   }

   public void SetStrategy(IResourceStrategy strategy) {
      _resourceStrategy = strategy;
   }

   public IResourceStrategy? GetStrategy() {
      return _resourceStrategy;
   }

   public void GenerateResourcePreview(string path) {
      if (_resourceStrategy == null) throw new Exception("IResourceStrategy not defined in Context");
      _resourceStrategy.LoadResource(path);
   }

   public void Draw() {
      _resourceStrategy?.Draw();
   }

   public void Reset() {
      _resourceStrategy?.QueueFree();
   }
}