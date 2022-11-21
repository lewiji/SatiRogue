using Godot;
using Godot.Collections;

namespace SatiRogue.Resources;

public class ParticlesResourceSet : Resource
{
        [Export] public ParticlesMaterial ParticlesMaterial { get; set; }
        [Export] public Array<Mesh> DrawPassMeshes { get; set; }

        public ParticlesResourceSet()
        {
                ParticlesMaterial = default!;
                DrawPassMeshes = new Array<Mesh>();
        }

        public ParticlesResourceSet(ParticlesMaterial material, Mesh[] meshes)
        {
                ParticlesMaterial = material;
                DrawPassMeshes = new Array<Mesh>(meshes);
        }
}