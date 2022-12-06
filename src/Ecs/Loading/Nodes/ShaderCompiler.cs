using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Resources;

namespace SatiRogue.Ecs.Loading.Nodes;

public partial class ShaderCompiler : CanvasLayer {
   static readonly PackedScene SpatialWigglerScene = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/SpatialShaderWiggler.tscn");
   static readonly PackedScene ParticlesWiggler = GD.Load<PackedScene>("res://src/Ecs/Loading/Nodes/ParticlesWiggler.tscn");

   Node3D _spatialRoot = default!;
   
   MultiMeshInstance3D? _multiMeshInstance;
   GPUParticles3D? _particles;
   MeshInstance3D? _meshInstance;

   Queue<StandardMaterial3D> _spatialMaterials = new();
   Queue<ShaderMaterial> _shaderMaterials = new();
   Queue<Mesh> _meshes = new();
   Queue<Mesh> _multiMeshes = new();
   Queue<ParticlesResourceSet> _particlesMaterials = new();

   const float TimeToWiggle = 0.5f;

   public override void _Ready()
   {
	   _spatialRoot = GetNode<Node3D>("%SpatialRoot");
	   InstanceWigglers();
   }

   void InstanceWigglers()
   {
      InstanceParticlesWiggler();
      InstanceSpatialWiggler();
      InstanceMultiMeshWiggler();
   }
   
   public bool ProcessResourcePreloader(Resource res) {
      if (res is Mesh mesh) {
         if (res.ResourceName == "dungeon_voxel") {
            _multiMeshes.Enqueue(mesh);
         }
         else {
            _meshes.Enqueue(mesh);
         }
      }
      if (res is not Material material)
         return false;

      switch (material) {
         case StandardMaterial3D spatialMaterial:
            _spatialMaterials.Enqueue(spatialMaterial);
            break;
         case ShaderMaterial shaderMaterial:
            _shaderMaterials.Enqueue(shaderMaterial);
            break;
         case ParticleProcessMaterial particlesMaterial:
            _particlesMaterials.Enqueue(new ParticlesResourceSet{ParticleProcessMaterial = particlesMaterial});
            break;
      }
      return true;
   }

   public override void _PhysicsProcess(double delta)
   {
      if (_multiMeshes.Count > 0 && _multiMeshInstance?.Multimesh == null)
      {
         var mesh = _multiMeshes.Dequeue();
         SetMultiMesh(mesh);
      } else if (_meshes.Count > 0 && _meshInstance?.Mesh == null)
      {
         var mesh = _meshes.Dequeue();
         SetMesh(mesh);
      } else if (_spatialMaterials.Count > 0 && _meshInstance?.Mesh == null)
      {
         var mat = _spatialMaterials.Dequeue();
         SetMeshMaterial(mat);
      } else if (_particlesMaterials.Count > 0 && _particles?.ProcessMaterial == null)
      {
         var particleMat = _particlesMaterials.Dequeue();
         SetParticles(particleMat);
      }
   }

   void InstanceMultiMeshWiggler() {
      Logger.Info("Instancing multimesh wiggler");
      var mmInst = new MultiMeshInstance3D();
      _spatialRoot.AddChild(mmInst);
      _multiMeshInstance = mmInst;
   }

   async void SetMultiMesh(Mesh mesh)
   {
      if (_multiMeshInstance == null) return;
      Logger.Info("Precompiling multimesh");
      var mMesh = new MultiMesh();
      _multiMeshInstance.Multimesh = mMesh;
      mMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
      mMesh.UseCustomData = true;
      _multiMeshInstance.Multimesh.Mesh = mesh;
      _multiMeshInstance.Multimesh.InstanceCount = 5;

      for (var i = 0; i < 5; i++)
      {
         _multiMeshInstance.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3(i, i, i)));
         _multiMeshInstance.Multimesh.SetInstanceCustomData(i, Color.Color8((byte)i, 0, 0, 0));
      }

      await ToSignal(GetTree().CreateTimer(TimeToWiggle), "timeout");
      _multiMeshInstance.Multimesh = null;
   }

   void InstanceSpatialWiggler() {
      var spatialWiggler = SpatialWigglerScene.Instantiate<Node3D>();
      spatialWiggler.Position = new Vector3((float) GD.RandRange(-5, 5), (float) GD.RandRange(-5, 5), (float) GD.RandRange(0, 20));
      _spatialRoot.AddChild(spatialWiggler);
      _meshInstance = spatialWiggler.GetNode<MeshInstance3D>("MeshInstance3D");
   }

   async void SetMesh(Mesh mesh)
   {
      if (_meshInstance != null)
      {
         _meshInstance.Mesh = mesh;
         
         await ToSignal(GetTree().CreateTimer(TimeToWiggle), "timeout");
         _meshInstance.Mesh = null;
      }
   }

   async void SetMeshMaterial(Material material)
   {
      if (_meshInstance != null)
      {
         _meshInstance.Mesh = new BoxMesh();
         _meshInstance.MaterialOverride = material;
         
         await ToSignal(GetTree().CreateTimer(TimeToWiggle), "timeout");
         _meshInstance.Mesh = null;
      }
   }


   async void InstanceParticlesWiggler()
   {
      var particlesWiggler = ParticlesWiggler.Instantiate<Node3D>();
      _particles = particlesWiggler.GetNode<GPUParticles3D>("GPUParticles3D");
      _spatialRoot.AddChild(particlesWiggler);
   }

   async void SetParticles(ParticlesResourceSet particlesResources)
   {
      if (_particles == null) return;
      _particles.ProcessMaterial = particlesResources.ParticleProcessMaterial;
      _particles.DrawPasses = Math.Min(4, particlesResources.DrawPassMeshes.Count);
      for (var i = 1; i < Math.Min(4, particlesResources.DrawPassMeshes.Count); i++)
      {
         _particles.Set($"draw_pass_{i}", particlesResources.DrawPassMeshes[i]);
      }

      _particles.Emitting = true;
      
      await ToSignal(GetTree().CreateTimer(TimeToWiggle), "timeout");
      _particles.ProcessMaterial = null;
      _particles.DrawPasses = 0;
   }
}