tool
extends Control
# Signals
signal on_images_generated(userFolder)
# Options controls
onready var normal_check: CheckButton = get_node("%NormalCheck")
onready var specular_check: CheckButton = get_node("%SpecularCheck")
onready var occlusion_check: CheckButton = get_node("%OcclusionCheck")
onready var parallax_check: CheckButton = get_node("%ParallaxCheck")
onready var preset_menu: MenuButton = get_node("%PresetMenu")
# Laigter preset load dialog
onready var file_dialog: FileDialog = get_node("%FileDialog")
# Drag & Drop/Image Preview Controls
onready var image_panel: Panel = get_node("%Panel")
onready var image_preview: TextureRect = get_node("%ImagePreview")
onready var drop_target: Control = get_node("%DropContainer")
# Generate button
onready var generate_button: Button = get_node("%GenerateMaps")
# The Texture resource to be used as input to Laigter
var input_texture: Texture
var image_file_extensions = ResourceLoader.get_recognized_extensions_for_type("Image")

const LaigterSettings = preload("LaigterSettings.gd")

onready var settings_buttons = {
   LaigterSettings.PROJECT_SETTING.GENERATE_NORMAL_MAP: normal_check, 
   LaigterSettings.PROJECT_SETTING.GENERATE_SPECULAR: specular_check, 
   LaigterSettings.PROJECT_SETTING.GENERATE_OCCLUSION: occlusion_check, 
   LaigterSettings.PROJECT_SETTING.GENERATE_PARALLAX: parallax_check
}

var settings_laigterflags = {
   LaigterSettings.PROJECT_SETTING.GENERATE_NORMAL_MAP: "-n", 
   LaigterSettings.PROJECT_SETTING.GENERATE_SPECULAR: "-c", 
   LaigterSettings.PROJECT_SETTING.GENERATE_OCCLUSION: "-o", 
   LaigterSettings.PROJECT_SETTING.GENERATE_PARALLAX: "-p"
}

func _ready():
   generate_button.connect("pressed", self, "on_generate_pressed")

func setup_options(settingsObj: LaigterSettings):
   for setting_idx in settings_buttons:
      settings_buttons[setting_idx].pressed = ProjectSettings.get_setting(LaigterSettings.get_qualified_setting_name(setting_idx))
      settings_buttons[setting_idx].connect("toggled", self, "on_pref_toggled", [setting_idx])

func on_pref_toggled(button_pressed: bool, setting_idx: int):
   ProjectSettings.set_setting(LaigterSettings.get_qualified_setting_name(setting_idx), button_pressed)
   
# get a string representing the optional flags to pass to the laigter binary from settings
func get_cmd_flags() -> String:
   var flags = PoolStringArray()
   for setting_idx in settings_buttons:
      var checkButton = settings_buttons[setting_idx]
      if checkButton != null and checkButton.pressed:
         flags.append(settings_laigterflags[setting_idx])
   return flags.join(" ")
   
#   └─▪ laigter --help
#   Usage: laigter [options]
#   Test helper
#
#   Options:
#   -h, --help                            Displays help on commandline options.
#   --help-all                            Displays help including Qt specific
#   options.
#   -v, --version                         Displays version information.
#   -s, --software-opengl                 Use software opengl renderer.
#   -g, --no-gui                          do not start graphical interface
#   -d, --diffuse <diffuse texture path>  diffuse texture to load
#   -n, --normal                          generate normals
#   -c, --specular                        generate specular
#   -o, --occlusion                       generate occlusion
#   -p, --parallax                        generate parallax
#   -r, --preset <preset file path>       presset to load

func on_generate_pressed():
   if (input_texture != null):
      var filehash = input_texture.resource_path.hash()
      var user_tex_cache_dir = get_cache_resource_path(input_texture, filehash)
      var cached_texture_file_path = "%s/%s" % [user_tex_cache_dir, input_texture.resource_path.get_file()]  
      var dir = Directory.new()
      
      assert(dir.file_exists(input_texture.resource_path), "failed find source texture at %s" % input_texture.resource_path)
      assert(dir.dir_exists(user_tex_cache_dir) or dir.make_dir_recursive(user_tex_cache_dir) == OK, "failed to create cache dir: %s" % user_tex_cache_dir)
      assert(dir.copy(input_texture.resource_path, cached_texture_file_path) == OK, "failed to copy texture resource to tmp user:// dir. Error %d")
      print("copying '%s' from\n'%s' to '%s'" % [input_texture.resource_path.get_file(), input_texture.resource_path.get_base_dir(), cached_texture_file_path])
      
      var cmd = "/usr/bin/laigter --no-gui %s -d '%s'" % [get_cmd_flags(), cached_texture_file_path]
      if (execute_cmd(cmd) == 0):
         emit_signal("on_images_generated", "user://%d" % filehash)
      
      
func get_cache_resource_path(input_texture: Resource, dir: int):
   var path = "%s/%d" % [OS.get_user_data_dir(), dir]
   print("cache: %s" % path)
   return path   
      
func execute_cmd(cmd: String) -> int:
      var output = []
      print("$ %s" % cmd)
      
      var exit_code = OS.execute("/usr/bin/bash", ["-c", cmd], true, output, true)
      print("Process exited with code: %d" % exit_code)
      for line in output:
         print(line)
      return exit_code

# tell godot we accept drag and drop operations, validate the type of data 
# dropped is supported and check the drop was over our drop zone
func can_drop_data(position: Vector2, data):
   var valid_file = typeof(data) == TYPE_DICTIONARY and data["type"] == "files"
   var over_texture_panel = drop_target.get_rect().has_point(position);
   # visual effect on hover with valid data (darken on hover)
   image_panel.get_stylebox("panel").shadow_size = 1 if valid_file and over_texture_panel else 0         
   return valid_file and over_texture_panel

# handle dropped data - we've got a file. check we can handle it, and then set 
# it as our input image
func drop_data(position, data):
   image_panel.get_stylebox("panel").shadow_size = 0
   var files: Array = data["files"]
   for file in files:
      if image_file_extensions.has(file.get_extension()) and ResourceLoader.exists(file):
         set_input_image(file)

# given a Resource, get the full OS file path from the res:// path
func get_absolute_resource_path(res_path: String):
   var filesystem = File.new()
   var err = filesystem.open(res_path, File.READ)
   filesystem.call_deferred("close")
   filesystem.call_deferred("free")
   
   if (err != OK):
      print("%s: Failed load resource from %s" % [err, res_path])
      return null
   
   var abs_path = filesystem.get_path_absolute()
   return abs_path

func set_input_image(path: String):
   input_texture = load(path)
   image_preview.texture = input_texture
   if image_preview.texture != null:
      image_panel.get_node("Label").visible = false
   else:
      image_panel.get_node("Label").visible = true
