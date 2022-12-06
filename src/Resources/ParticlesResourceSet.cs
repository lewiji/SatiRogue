using Godot;
using Godot.Collections;

namespace SatiRogue.Resources;

public partial class ParticlesResourceSet : Resource
{
        [Export] public ParticleProcessMaterial ParticleProcessMaterial { get; set; }
        [Export] public Array<Mesh> DrawPassMeshes { get; set; }

        public ParticlesResourceSet()
        {
                ParticleProcessMaterial = default!;
                DrawPassMeshes = new Array<Mesh>();
        }

        public ParticlesResourceSet(ParticleProcessMaterial material, Mesh[] meshes)
        {
                ParticleProcessMaterial = material;
                DrawPassMeshes = new Array<Mesh>(meshes);
        }
}