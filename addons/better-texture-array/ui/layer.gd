tool
extends Button

enum Channels {RED, GREEN, BLUE, ALPHA, ALL}
export(TextureLayered) var texture: TextureLayered setget set_texture
export(int) var index = 0 setget set_index
export(Channels) var channel = Channels.ALL setget set_channel

var viewer
var index_label
var edit_dialog: EditorFileDialog

signal update_layer

func set_texture(v: TextureLayered):
	texture = v
	var is_3d = v is Texture3D
	viewer.material.set_shader_param("is_3d", is_3d)
	viewer.material.set_shader_param("tex3d" if is_3d else "texarr", v)

func set_index(v: int):
	index = v
	index_label.text = str(v)
	viewer.material.set_shader_param("idx", v)

func set_channel(v: int):
	channel = v
	var chn = Color(1, 1, 1, 1)
	if v < Channels.ALL:
		chn = Color(0, 0, 0, 0)
		chn[v] = 1
	viewer.material.set_shader_param("chn", chn)

func _init():
	toggle_mode = true
	size_flags_vertical = SIZE_EXPAND_FILL
	viewer = ColorRect.new()
	viewer.mouse_filter = MOUSE_FILTER_IGNORE
	viewer.material = ShaderMaterial.new()
	viewer.material.shader = preload("res://addons/better-texture-array/ui/layer.shader")
	viewer.rect_min_size = Vector2(64, 64)
	index_label = Label.new()
	index_label.rect_position = Vector2(4, 4)
	
	add_child(viewer)
	add_child(index_label)

func _notification(what):
	if what == NOTIFICATION_RESIZED:
		var pad = 4
		var dim = rect_size.y - pad
		var off = Vector2((rect_size.x - dim) / 2.0, 2)
		viewer.rect_size = Vector2(dim, dim)
		viewer.rect_position = off

func _toggled(pressed: bool):
	if pressed:
		texture.set_meta("layer_selected", index)
		texture.emit_signal("changed")

func _input(event):
	if is_hovered() and event is InputEventMouseButton and event.doubleclick:
		edit_dialog.popup_for_layer(self)

func update_layer(img, img_chn):
	emit_signal("update_layer", img, index, img_chn, channel)
