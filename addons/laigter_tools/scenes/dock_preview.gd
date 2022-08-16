tool
extends Control

signal on_images_saved
signal on_reset

var cli_result: LaigterCliResult
var texture_paths = []
var imported_textures = {}
var save_textures = {}
var save_spatial_material = false
var spatial_material: SpatialMaterial
var modified_times: Dictionary
var editor_filesystem: EditorFileSystem
onready var grid: GridContainer = get_node("%PreviewGrid")
onready var btn_save: Button = get_node("%SaveImages")
onready var btn_reset: Button = get_node("%Reset")
onready var scan_changes_timer: Timer = get_node("ScanForChanges")

func _ready():
	btn_save.connect("pressed", self, "on_save_requested")
	btn_reset.connect("pressed", self, "reset")
	connect("visibility_changed", self, "on_visibility_changed")
	scan_changes_timer.connect("timeout", self, "on_scan_changes")
	
# pause/resume file watches when panel shown/hidden
func on_visibility_changed():
	if (scan_changes_timer == null):
		return
	if is_visible_in_tree():
		if (imported_textures.size() > 0 and scan_changes_timer.is_stopped()):
			scan_changes_timer.start(2)
	else:
		if (!scan_changes_timer.is_stopped()):
			scan_changes_timer.stop()

# laigter cli has generated images, reset everything and load them
func on_images_generated(result: LaigterCliResult):
	cli_result = result
	reset(false)
	load_preview_images_from_dir(cli_result)
	scan_changes_timer.start(2)

# check cached modified times for loaded textures. if changed, reload that texture
func on_scan_changes():
	if (cli_result == null):
		return
	var file = File.new()
	for tex_path in texture_paths:
		if file.open(tex_path, File.READ) != OK:
			continue
		var filename = tex_path.get_file()
		var modified = file.get_modified_time(tex_path)
		if (modified_times[filename] != modified):
			modified_times[filename] = modified
			if (imported_textures.has(filename)):
				var img = Image.new()
				if (img.load(tex_path) == OK):
					imported_textures[filename].create_from_image(img, Texture.FLAG_ANISOTROPIC_FILTER | Texture.FLAG_MIPMAPS)
					print("Reloaded texture: %s" % [tex_path])
	file.close()

func on_save_requested():
	assert(cli_result != null, "No Laigter results object found, couldn't save")
	for texture in imported_textures:
		if (save_textures[texture]):
			var new_file_name = "%s/%s" % [cli_result.input_file.get_base_dir(), imported_textures[texture].resource_name]
			ResourceSaver.save(new_file_name, imported_textures[texture], ResourceSaver.FLAG_CHANGE_PATH)
			
	emit_signal("on_images_saved")
	
	yield(editor_filesystem, "resources_reimported")
	
	if (save_spatial_material and spatial_material != null):
		for texture in imported_textures:
			if (save_textures[texture]):
				if (save_spatial_material and spatial_material != null):
					var new_file_name = "%s/%s" % [cli_result.input_file.get_base_dir(), imported_textures[texture].resource_name]
					var new_texture = load(new_file_name)
					set_spatial_material_texture(spatial_material, new_texture, texture)
		var filename = cli_result.input_file.get_file()
		var file_no_extension = filename.get_slice(".", filename.count(".") - 1)
		# TODO this saves the material with embedded resources, rather than the filesystem resources we just saved
		ResourceSaver.save("%s/%s_spatial_material.tres" % [cli_result.input_file.get_base_dir(), file_no_extension], spatial_material)
		emit_signal("on_images_saved")
	
# when "propagate" is true, emit signal allowing other scenes to reset too
func reset(propagate: bool = true):
	if(!scan_changes_timer.is_stopped()):
		scan_changes_timer.stop()
	var kids = grid.get_children()
	for kid in kids:
		grid.remove_child(kid)
		kid.free()
	texture_paths.clear()
	imported_textures.clear()
	save_textures.clear()
	modified_times.clear()
	save_spatial_material = false
	spatial_material = null
	if (propagate):
		emit_signal("on_reset")
	
func enumerate_textures_from_dir(path: String):
	var dir = Directory.new()	
	assert(dir.open(path) == OK, "Couldn't open directory: " + path)
	texture_paths.clear()
	dir.list_dir_begin()
	var file_name = dir.get_next()
	while file_name != "":
		if !dir.current_is_dir():
			texture_paths.append("%s/%s" % [path, file_name])
		file_name = dir.get_next()
	dir.list_dir_end()
	
func on_save_changed(checked, filename):
	save_textures[filename] = checked
	
func on_save_mat_changed(checked):
	save_spatial_material = checked

func add_texture_preview(tex: Texture, filename: String):
	var preview = preload("preview_image.tscn").instance()
	preview.get_node("%TextureRect").texture = tex
	preview.get_node("%FilePath").text = filename
	preview.connect("save_changed", self, "on_save_changed", [filename])
	save_textures[filename] = true
	grid.add_child(preview)
	
func set_spatial_material_texture(mat: SpatialMaterial, tex: Texture, filename: String):
	var file_no_extension = filename.get_slice(".", filename.count(".") - 1)
	var suffix = file_no_extension.get_slice("_", file_no_extension.count("_"))
	match (suffix):
		"n":
			 mat.normal_texture = tex
			 mat.normal_enabled = true
			 mat.normal_scale = 1
		"s":
			 mat.roughness_texture = tex
			 mat.roughness = 1
		"o":
			 mat.ao_texture = tex
			 mat.ao_enabled = 1
			 mat.ao_light_affect = 0.7
		"p":
			 mat.depth_texture = tex
			 mat.depth_enabled = true
			 mat.depth_scale = -0.025
		_:
			mat.albedo_texture = tex
			
func texture_load_image(filename: String, tex_path: String, tex: ImageTexture, img: Image, file: File):
	tex.storage = ImageTexture.STORAGE_COMPRESS_LOSSLESS
	tex.create_from_image(img, Texture.FLAG_ANISOTROPIC_FILTER | Texture.FLAG_REPEAT | Texture.FLAG_MIPMAPS)
	tex.resource_name = filename
	imported_textures[filename] = tex
	modified_times[filename] = file.get_modified_time(tex_path)

func create_texture_from_image(tex_path: String, mat: SpatialMaterial, file: File):
	var tex = ImageTexture.new()
	var img = Image.new()
	var filename = tex_path.get_file()
	if (img.load(tex_path) == OK):
		texture_load_image(filename, tex_path, tex, img, file)
		add_texture_preview(tex, filename)
		set_spatial_material_texture(mat, tex, filename)
	else:
		print("Unsupported file: %s" % tex_path)

func load_preview_images_from_dir(cli_result: LaigterCliResult):
	enumerate_textures_from_dir(cli_result.cache_dir)
	var lit_preview = preload("preview_lit.tscn").instance()
	var mat = SpatialMaterial.new()
	mat.params_alpha_scissor_threshold = 0.1
	mat.params_use_alpha_scissor = true
	mat.params_cull_mode = SpatialMaterial.CULL_DISABLED
	var file = File.new()
	for tex_path in texture_paths:
		create_texture_from_image(tex_path, mat, file)
	spatial_material = mat
	lit_preview.get_node("%PreviewSpatial/MeshInstance").material_override = mat
	lit_preview.connect("save_spatial_mat_changed", self, "on_save_mat_changed")
	file.close()
	grid.add_child(lit_preview)
