using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.addons.src.WfcDock;
using SatiRogue.addons.wfc_tools.src.WfcDock;

namespace SatiRogue.addons.wfc_tools.src.ResourceStrategy; 

public abstract class AbstractResourceStrategy : Node, IResourceStrategy {
        private readonly PackedScene _previewContainerScene =
            GD.Load<PackedScene>("res://addons/wfc_tools/scenes/WfcDock/PreviewContainer.tscn");
        protected readonly EditorInterface TheEditorInterface = new EditorPlugin().GetEditorInterface();
        public abstract Resource? LoadResource(string path);
        private const int ResourcesPerPage = 64;
        public List<Resource> Resources { get; set; } = new();
        public List<ResourcePreviewContainer> PreviewContainersToProcess { get; set; } = new();
        public Queue<ResourcePreviewContainer> QueuedResources { get; set; } = new();
        private bool _dirty;

        protected abstract void HandlePreviewForContainer(ResourcePreviewContainer previewContainer);

        protected void OnPreviewGenerated(string path, Texture preview, Texture thumbnailPreview, object userdata) {
            var previewContainer = (ResourcePreviewContainer) userdata;
            previewContainer.SetPreviewTexture(preview);
        }
        
        public void Draw() {
            var someMeshes = Resources.Take(ResourcesPerPage);
            foreach (var mesh in someMeshes) {
                var preview = _previewContainerScene.Instance<wfc_tools.src.WfcDock.ResourcePreviewContainer>();
                ResourcePanel.PreviewGrid.AddChild(preview);
                preview.SetResource(mesh);
                PreviewContainersToProcess.Add(preview);
                if (PreviewContainersToProcess.Count == ResourcesPerPage - 1)
                    preview.CallDeferred(nameof(wfc_tools.src.WfcDock.ResourcePreviewContainer.EnableAreaMonitoring));
            }

            Resources.RemoveRange(0, ResourcesPerPage);
            CreatePreviewJobStack();
        }
        
        private void CreatePreviewJobStack() {
            var unpreviewed = 
                PreviewContainersToProcess.Where(s => !s.GeneratedPreview);

            var meshPreviewContainers = unpreviewed.ToList();
            foreach (var meshPreviewContainer in meshPreviewContainers) {
                if (meshPreviewContainer.TheResource == null) continue;
                QueuedResources.Enqueue(meshPreviewContainer);
            }

            PreviewContainersToProcess.Clear();
        }
        
        public override void _Process(float delta) {
            if (QueuedResources.Count > 0) {
                var previewContainer = QueuedResources.Dequeue();
                HandlePreviewForContainer(previewContainer);
                _dirty = true;
            } else if (_dirty) {
                WfcSignals.Emit(nameof(WfcSignals.EnableFolderButton));
            }
        }
}