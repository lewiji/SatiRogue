tool
extends Control

var textures = []
onready var grid: GridContainer = get_node("%PreviewGrid")
onready var checkbox_orig_dir: CheckBox = get_node("%CheckOriginalDir")
onready var btn_save: Button = get_node("%SaveImages")

#func _ready():
   #connect("popup_hide", self, "on_hide")
   
func on_images_generated(userCacheFolder: String):
   print(userCacheFolder)
   load_preview_images_from_dir(userCacheFolder)
   EditorPlugin.new().make_bottom_panel_item_visible(self)
   
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
   
   var lit_preview = preload("PreviewLit.tscn").instance()
   var mat = lit_preview.get_node("%PreviewSpatial/MeshInstance").material_override as SpatialMaterial
   
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
      img.generate_mipmaps()
      
      tex.create_from_image(img, Texture.FLAG_ANISOTROPIC_FILTER | Texture.FLAG_MIPMAPS)
      var preview = preload("res://addons/generate_laigter_maps/PreviewImage.tscn").instance()
      preview.get_node("%TextureRect").texture = tex
      var slices = tex_path
      preview.get_node("%FilePath").text = filename
      grid.add_child(preview)
      
      var filename_no_ext = filename.get_slice(".", filename.count(".") - 1)
      if filename_no_ext.ends_with("_n"):
         mat.normal_texture = tex
         mat.normal_enabled = true
         mat.normal_scale = 1
      elif filename_no_ext.ends_with("_s"):
         mat.roughness_texture = tex
         mat.roughness = 1
      elif filename_no_ext.ends_with("_o"):
         mat.ao_texture = tex
         mat.ao_enabled = 1
         mat.ao_light_affect = 0.5
      elif filename_no_ext.ends_with("_p"):
         mat.depth_texture = tex
         mat.depth_enabled = true
         mat.depth_deep_parallax = true
         mat.depth_scale = 0.05
      else:
         mat.albedo_texture = tex
         
      grid.add_child(lit_preview)
      yield(get_tree(), "idle_frame")
      lit_preview.get_node("%PreviewSpatial/AnimationPlayer").play("spin")
   
