tool
extends Control

var textures = []
onready var grid: GridContainer = get_node("%PreviewGrid")

#func _ready():
   #connect("popup_hide", self, "on_hide")
   
func on_images_generated(userCacheFolder: String):
   print(userCacheFolder)
   load_preview_images_from_dir(userCacheFolder)
   
func on_hide():
   var kids = grid.get_children()
   for kid in kids:
      kid.queue_free()
   textures.clear()

func load_preview_images_from_dir(dir_path: String):
   var dir = Directory.new()
   if (!dir.dir_exists(dir_path)):
      print("No directory: " + dir_path)
      return
   
   if (dir.open(dir_path) != OK):
      print("No open directory: " + dir_path)
      return
      
   if (dir.list_dir_begin() != OK):
      print("Coudln't list: " + dir_path)
      return
      
   var file_name = dir.get_next()
   
   while file_name != "":
      if !dir.current_is_dir():
         print("Found file: " + file_name)
         textures.append("%s/%s" % [dir_path, file_name])
      file_name = dir.get_next()
   
   #var filesystem = File.new()
   for tex_path in textures:
      #if filesystem.open(tex_path, File.READ) != OK:
      #   print("couldn't open file at %s" % tex_path)
      #   return  
      var file = File.new()
      var str_path  = tex_path as String
      var filename = str_path.get_slice("/", str_path.count("/"))
      var tex = ImageTexture.new()
      var img = Image.new()
      img.load(tex_path)
      tex.create_from_image(img)
      var preview = preload("res://addons/generate_laigter_maps/PreviewImage.tscn").instance()
      preview.get_node("%TextureRect").texture = tex
      var slices = tex_path
      preview.get_node("%FilePath").text = filename
      grid.add_child(preview)
