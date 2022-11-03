tool
extends EditorProperty

const Layer = preload("res://addons/better-texture-array/ui/layer.gd")
const CreateDialog = preload("res://addons/better-texture-array/ui/create_dialog.tscn")
const EditDialog = preload("res://addons/better-texture-array/ui/edit_dialog.gd")
var undo_redo: UndoRedo
var create_button
var create_dialog
var edit_dialog
var layer_box
var layer_list
var layer_group
var toolbar
var tbb_vis
var tbb_sep
var tbb_grp
var tbb_red
var tbb_grn
var tbb_blu
var tbb_alp
var tbb_all

func _init():
	label = "Layers"
	create_button = Button.new()
	create_button.text = "Create"
	create_dialog = CreateDialog.instance()
	edit_dialog = EditDialog.new()

	layer_box = VBoxContainer.new()
	layer_list = VBoxContainer.new()
	layer_group = ButtonGroup.new()

	toolbar = HBoxContainer.new()
	tbb_vis = CheckButton.new()
	tbb_sep = VSeparator.new()
	tbb_grp = ButtonGroup.new()
	tbb_red = Button.new()
	tbb_grn = Button.new()
	tbb_blu = Button.new()
	tbb_alp = Button.new()
	tbb_all = Button.new()

	toolbar.alignment = BoxContainer.ALIGN_CENTER
	tbb_sep.size_flags_horizontal = SIZE_EXPAND_FILL
	tbb_sep.add_stylebox_override("separator", StyleBoxEmpty.new())
	tbb_vis.toggle_mode = true
	tbb_red.toggle_mode = true
	tbb_grn.toggle_mode = true
	tbb_blu.toggle_mode = true
	tbb_alp.toggle_mode = true
	tbb_all.toggle_mode = true
	tbb_vis.text = "Show"
	tbb_red.icon = preload("res://addons/better-texture-array/icons/RED.svg")
	tbb_grn.icon = preload("res://addons/better-texture-array/icons/GREEN.svg")
	tbb_blu.icon = preload("res://addons/better-texture-array/icons/BLUE.svg")
	tbb_alp.icon = preload("res://addons/better-texture-array/icons/ALPHA.svg")
	tbb_all.icon = preload("res://addons/better-texture-array/icons/ALL.svg")
	tbb_red.group = tbb_grp
	tbb_grn.group = tbb_grp
	tbb_blu.group = tbb_grp
	tbb_alp.group = tbb_grp
	tbb_all.group = tbb_grp
	tbb_all.pressed = true

	tbb_red.connect("toggled", self, "_toggle_channel", [Layer.Channels.RED])
	tbb_grn.connect("toggled", self, "_toggle_channel", [Layer.Channels.GREEN])
	tbb_blu.connect("toggled", self, "_toggle_channel", [Layer.Channels.BLUE])
	tbb_alp.connect("toggled", self, "_toggle_channel", [Layer.Channels.ALPHA])
	tbb_all.connect("toggled", self, "_toggle_channel", [Layer.Channels.ALL])

	toolbar.add_child(tbb_vis)
	toolbar.add_child(tbb_sep)
	toolbar.add_child(tbb_red)
	toolbar.add_child(tbb_grn)
	toolbar.add_child(tbb_blu)
	toolbar.add_child(tbb_alp)
	toolbar.add_child(tbb_all)
	toolbar.add_constant_override("separation", 0)

	layer_list.visible = false
	layer_box.add_child(toolbar)
	layer_box.add_child(layer_list)

	tbb_vis.connect("toggled", self, "_toggle_layers")
	create_button.connect("pressed", self, "_open_create_dialog")
	create_dialog.connect("acknowledged", self, "_do_create_texarr")

func update_list():
	var texarr = get_edited_object()
	var children = layer_list.get_children()
	var have = children.size()
	var want = texarr.get_depth() if texarr else 0
	layer_list.rect_min_size.y = want * 192

	for i in have:
		var layer = children[i]
		layer_list.remove_child(layer)
		layer.disconnect("update_layer", self, "update_texarr")
		layer.queue_free()

	for i in want:
		var layer = Layer.new()
		layer.edit_dialog = edit_dialog
		layer_list.add_child(layer)
		layer.index = i
		layer.texture = texarr
		layer.rect_min_size = Vector2(128, 128)
		layer.group = layer_group
		layer.connect("update_layer", self, "update_texarr")

func _ready():
	label = "Layers"
	add_child(create_button)
	add_child(layer_box)
	add_child(create_dialog)
	add_child(edit_dialog)
	set_bottom_editor(layer_box)

func update_property():
	# Reset the visible state of the layers UI from the meta value `layers_visible`
	var texarr = get_edited_object()
	var vis = texarr.has_meta("layers_visible") and texarr.get_meta("layers_visible")
	layer_list.visible = vis
	tbb_vis.pressed = vis
	
	update_list()

func create_texarr(width: int, height: int, depth: int, format: int, flags: int = Texture.FLAGS_DEFAULT):
	var texarr: TextureLayered = get_edited_object()
	var data = {"width": width, "height": height, "depth": depth, "format": format, "flags": flags, "layers": []}
	var img = Image.new()
	img.create(width, height, true, format)
	img.fill(Color.white)
	img.generate_mipmaps()
	for i in depth:
		data["layers"].append(img)
	emit_changed("data", data)
#	texarr.property_list_changed_notify()

func update_texarr(src: Image, idx: int, src_chn: int, dst_chn: int):
	var texarr: TextureLayered = get_edited_object()
	var prv = texarr.get_layer_data(idx)
	undo_redo.create_action("Update layer")
	undo_redo.add_do_method(get_script(), "_update_layer", texarr, src, idx, src_chn, dst_chn)
	undo_redo.add_undo_method(get_script(), "_update_layer", texarr, prv, idx, src_chn, dst_chn)
	undo_redo.commit_action()

static func _update_layer(texarr: TextureLayered, src, idx: int, chn_src: int = Layer.Channels.ALL, chn_dst: int = Layer.Channels.ALL):
	var dst: Image
	var size = Vector2(texarr.get_width(), texarr.get_height())

	if src is Texture:
		src = src.get_data()
	if src.get_size() != size:
		src.resize(size.x, size.y)
	if src.is_compressed():
		src.decompress()
	if src.get_format() != texarr.get_format():
		src.convert(texarr.get_format())

	if chn_src == Layer.Channels.ALL or chn_dst == Layer.Channels.ALL:
		dst = src
	else:
		dst = texarr.get_layer_data(idx)
		src.lock()
		dst.lock()
		for y in size.y:
			for x in size.x:
				var clr = dst.get_pixel(x, y)
				clr[chn_dst] = src.get_pixel(x, y)[chn_src]
				dst.set_pixel(x, y, clr)
		dst.unlock()
		src.unlock()

	dst.generate_mipmaps()
	texarr.set_layer_data(dst, idx)
	texarr.property_list_changed_notify()

func _open_create_dialog():
	create_dialog.popup_centered()

func _do_create_texarr(ok, vals):
	create_texarr(vals[0], vals[1], vals[2], vals[3], vals[4])

func _toggle_layers(visible: bool):
	# Store the layers visible state in meta for use on UI reload in `update_property`
	get_edited_object().set_meta("layers_visible", visible)
	layer_list.visible = visible

func _toggle_channel(visible: bool, chn: int):
	for layer in layer_list.get_children():
		layer.channel = chn
