using Godot;

namespace SatiRogue.scenes; 

public class RoomMultiMesh : MultiMeshInstance {
   [Export] int Width { get; set; } = 5;
   [Export] int Height { get; set; } = 5;
   
   public override void _Ready() {
      Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
      Multimesh.CustomDataFormat = MultiMesh.CustomDataFormatEnum.Data8bit;
      Multimesh.InstanceCount = CalculateInstanceCount();
      CreateRoom();
   }

   void CreateRoom() {
      var instanceId = 0;
      for (var y = 0; y < Height; y++) {
         for (var x = 0; x < Width; x++) {
            Multimesh.SetInstanceCustomData(instanceId, Color.Color8(0, 0, 0, 0));
            Multimesh.SetInstanceTransform(instanceId++, new Transform(Basis.Identity, new Vector3(x, 0, y)));
         }
      }

      for (var wallE = 1; wallE <= Height; wallE++) {
         Multimesh.SetInstanceCustomData(instanceId, Color.Color8(1, 0, 0, 0));
         Multimesh.SetInstanceTransform(instanceId++, new Transform(Basis.Identity, new Vector3(0, 1, wallE)));
      }

      for (var wallW = 0; wallW < Height; wallW++) {
         Multimesh.SetInstanceCustomData(instanceId, Color.Color8(1, 0, 0, 0));
         Multimesh.SetInstanceTransform(instanceId++, new Transform(Basis.Identity, new Vector3(Width, 1, wallW)));
      }

      for (var wallN = 1; wallN <= Width; wallN++) {
         Multimesh.SetInstanceCustomData(instanceId, Color.Color8(1, 0, 0, 0));
         Multimesh.SetInstanceTransform(instanceId++, new Transform(Basis.Identity, new Vector3(wallN, 1, Height)));
      }

      for (var wallS = 0; wallS < Width; wallS++) {
         Multimesh.SetInstanceCustomData(instanceId, Color.Color8(1, 0, 0, 0));
         Multimesh.SetInstanceTransform(instanceId++, new Transform(Basis.Identity, new Vector3(wallS, 1, 0)));
      }
   }

   int CalculateInstanceCount() {
      int floor = Width * Height;
      int walls = Width * 2 + Height * 2;
      return floor + walls;
   }
}