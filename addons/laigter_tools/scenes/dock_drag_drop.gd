tool
extends Control
# Signals
signal on_images_generated(cli_result)

# Options controls
onready var normal_check: CheckBox = get_node("%NormalCheck")
onready var specular_check: CheckBox = get_node("%SpecularCheck")
onready var occlusion_check: CheckBox = get_node("%OcclusionCheck")
onready var parallax_check: CheckBox = get_node("%ParallaxCheck")
onready var preset_menu: MenuButton = get_node("%PresetMenu")
onready var show_gui_check: CheckButton = get_node("%GuiCheck")
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
var presets = []
var selected_preset = 0

onready var settings_buttons = {
	LTConfig.ConfigKeys.GENERATE_NORMAL_MAP: normal_check, 
	LTConfig.ConfigKeys.GENERATE_SPECULAR: specular_check, 
	LTConfig.ConfigKeys.GENERATE_OCCLUSION: occlusion_check, 
	LTConfig.ConfigKeys.GENERATE_PARALLAX: parallax_check, 
	LTConfig.ConfigKeys.HIDE_LAIGTER_GUI: show_gui_check
}

func _ready():
	setup_options()
	load_presets()
	generate_button.connect("pressed", self, "on_generate_pressed")

func setup_options():
	for setting_idx in settings_buttons:
		settings_buttons[setting_idx].pressed = LTConfig.get_config_value(setting_idx)
		settings_buttons[setting_idx].connect("toggled", self, "on_pref_toggled", [setting_idx])

func load_presets():
	preset_menu.get_popup().clear()
	var dir = Directory.new()
	dir.open(LTConfig.PRESETS_DEFAULT_PATH)
	dir.list_dir_begin()
	var file_name = dir.get_next()
	while file_name != "":
		if !dir.current_is_dir():
			preset_menu.get_popup().add_check_item(file_name, presets.size())
			presets.append(file_name)
		file_name = dir.get_next()
		
	selected_preset = 0
	if (presets.size() > 0):
		preset_menu.get_popup().set_item_checked(selected_preset, true)
	
	preset_menu.get_popup().connect("id_pressed", self, "on_preset_selected")
	
func on_preset_selected(id: int):
	if (id < presets.size()):
		selected_preset = id
	
	var preset_items = preset_menu.get_popup().get_item_count()
	for i in range(preset_items):
		preset_menu.get_popup().set_item_checked(i, true if selected_preset == i else false)

func on_generate_pressed():
	var result = LaigterCli.execute_laigter(input_texture, presets[selected_preset])
	if (LTConfig.get_config_value(LTConfig.ConfigKeys.HIDE_LAIGTER_GUI) != true or result.exit_code == 0):
		yield(get_tree(), "idle_frame")
		emit_signal("on_images_generated", result)
		
func on_reset():
	image_preview.texture = null
	input_texture = null
	image_panel.get_node("Label").visible = true
	
func on_pref_toggled(button_pressed: bool, setting_idx: int):
	LTConfig.set_config_value(setting_idx, button_pressed)

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

func set_input_image(path: String):
	input_texture = load(path)
	image_preview.texture = input_texture
	if image_preview.texture != null:
		image_panel.get_node("Label").visible = false
	else:
		image_panel.get_node("Label").visible = true
