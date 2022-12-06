using Godot;

namespace SatiRogue.Debug;

public partial class ItemIconsToSpriteFrames : Control
{
    public override void _Ready()
    {
        var sourceDir = DirAccess.Open("res://assets/item_icons/item_icons.sprites/");
        var destDir = DirAccess.Open("res://resources/items/sprite_frames/");
        sourceDir.IncludeNavigational = false;
        sourceDir.ListDirBegin();
        var path = sourceDir.GetNext();
        while (path != "")
        {
            if (sourceDir.CurrentIsDir())
            {
                destDir.MakeDir(path.ToLower());
                var imagesDir = DirAccess.Open($"res://assets/item_icons/item_icons.sprites/{path}");
                imagesDir.IncludeNavigational = false;
                imagesDir.ListDirBegin();
                var imagePath = imagesDir.GetNext();
                while (imagePath != "")
                {
                    if (!imagesDir.CurrentIsDir())
                    {
                        var spriteFrames = new SpriteFrames();
                        var texture = GD.Load<Texture2D>($"res://assets/item_icons/item_icons.sprites/{path}/{imagePath}");
                        spriteFrames.AddFrame("default", texture);
                        ResourceSaver.Save(spriteFrames, $"res://resources/items/sprite_frames/{path.ToLower()}/{imagePath.ToLower().Replace(" ", "_")}");
                    }
                    imagePath = imagesDir.GetNext();
                }
            }
            path = sourceDir.GetNext();
        }
    }
}