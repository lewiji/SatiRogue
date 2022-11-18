using Godot;

namespace SatiRogue.Debug;

public class ItemIconsToSpriteFrames : Control
{
    public override void _Ready()
    {
        var sourceDir = new Directory();
        sourceDir.Open("res://assets/item_icons/item_icons.sprites/");

        var destDir = new Directory();
        destDir.Open("res://resources/items/sprite_frames/");

        sourceDir.ListDirBegin(true);
        var path = sourceDir.GetNext();
        while (path != "")
        {
            if (sourceDir.CurrentIsDir())
            {
                destDir.MakeDir(path.ToLower());
                var imagesDir = new Directory();
                imagesDir.Open($"res://assets/item_icons/item_icons.sprites/{path}");
                imagesDir.ListDirBegin(true);
                var imagePath = imagesDir.GetNext();
                while (imagePath != "")
                {
                    if (!imagesDir.CurrentIsDir())
                    {
                        var spriteFrames = new SpriteFrames();
                        var texture = GD.Load<Texture>($"res://assets/item_icons/item_icons.sprites/{path}/{imagePath}");
                        spriteFrames.AddFrame("default", texture);
                        ResourceSaver.Save($"res://resources/items/sprite_frames/{path.ToLower()}/{imagePath.ToLower().Replace(" ", "_")}", spriteFrames);
                    }
                    imagePath = imagesDir.GetNext();
                }
            }
            path = sourceDir.GetNext();
        }
    }
}