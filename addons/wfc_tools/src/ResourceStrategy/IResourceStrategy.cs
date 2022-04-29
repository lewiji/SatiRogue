using System.Collections.Generic;
using Godot;
using SatiRogue.addons.wfc_tools.src.WfcDock;

namespace SatiRogue.addons.wfc_tools.src.ResourceStrategy;

public interface IResourceStrategy {
    void Draw();
    void QueueFree();
    Resource? LoadResource(string path);
    List<Resource> Resources { get; set; }
    List<ResourcePreviewContainer> PreviewContainersToProcess { get; set; }
    Queue<ResourcePreviewContainer> QueuedResources { get; set; }
}