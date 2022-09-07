tool
extends ImageTexture
class_name VersatileAtlasTexture, "res://addons/versatile_atlas/icon_x.svg"


### Versatile by design


const FALLBACK_IMAGE = preload("res://addons/versatile_atlas/fallback_image.tres")
const ALPHA_FIX_THERESHOLD = 0.0

export var atlas: Texture setget set_atlas
export var region: Rect2 setget set_region
export var margin: Rect2 setget set_margin
export var margin_color: Color = Color.transparent setget set_margin_color
export var repeat: bool = false setget set_repeat


func _init():
#	print("init start")
	update_image()
	
	# Remove possible meta laying around
	if Engine.editor_hint and has_meta("done_set"):
		remove_meta("done_set")
#	print("init end")


func set_atlas(val):
	atlas = val
	if atlas and not atlas.is_connected("changed", self, "update_image"):
		var __ = atlas.connect("changed", self, "update_image")
	update_image()


func set_region(val):
	region = val
	update_image()


func set_margin(val: Rect2):
	margin = Rect2(val.position.abs(), val.size.abs())
	update_image()


func set_margin_color(val):
	margin_color = val
	update_image()


func set_repeat(val):
	repeat = val
	if is_debug_running() and not has_meta("done_set"):
		set_meta("done_set", true)
	update_image()


func is_debug_running():
	return OS.is_debug_build() and not Engine.editor_hint


func update_image():
	if !atlas:
		set_invalid()
		return
	if region.size.x == 0 or region.size.y == 0:
		set_invalid()
		return
	
	assert(not(atlas is GradientTexture2D or atlas is CurveTexture), "Sorry, but VersatileAtlasTexture does not work with GradientTexture2D or CurveTexture")
	if OS.is_debug_build() and atlas is AnimatedTexture:
		push_warning("VersatileAtlasTexture does not work with AnimatedTexture")
	
	var p_flags = flags
	print(has_meta("done_set"))
#
#	if not atlas.get_data():
#		print(atlas)
#		for i in atlas.get_property_list():
#			if i.name in atlas:
#				print(i.name)
#				print(atlas[i.name])
#		breakpoint
#
	var p_image = atlas.get_data().get_rect(add_rects(region, margin))
	
	# False only if position and size are both Vector2.ZERO
	if margin.position or margin.size:
		# Easier to edit
		var arr_margin: Array = margin2arr(margin)
		var image_size: Vector2 = p_image.get_size()
		# Math madness
		if arr_margin[0]:
			p_image.fill_rect(Rect2(Vector2.ZERO, Vector2(arr_margin[0], image_size.y)), margin_color)
		if arr_margin[1]:
			p_image.fill_rect(Rect2(Vector2.ZERO, Vector2(image_size.x, arr_margin[1])), margin_color)
		if arr_margin[2]:
			p_image.fill_rect(Rect2(Vector2(image_size.x - arr_margin[2], 0), Vector2(arr_margin[2], image_size.y)), margin_color)
		if arr_margin[3]:
			p_image.fill_rect(Rect2(Vector2(0, image_size.y - arr_margin[3]), image_size), margin_color)
		
		if margin_color.a <= ALPHA_FIX_THERESHOLD:
			p_image.fix_alpha_edges()
	
	create_from_image(p_image)
	flags = p_flags
	update_repeat()
	emit_changed()


func add_rects(r1: Rect2, r2: Rect2) -> Rect2:
	return Rect2(r1.position - r2.position, r1.size + r2.size + r2.position)


func margin2arr(rect: Rect2) -> Array:
	return [rect.position.x, rect.position.y, rect.size.x, rect.size.y]


func update_repeat():
	if repeat:
		flags |= FLAG_REPEAT
	else:
		flags &= ~FLAG_REPEAT


func set_invalid():
	var data = get_data()
	
	if not data or data.get_data() != FALLBACK_IMAGE.get_data():
#		print("FALLBACK")
		create_from_image(FALLBACK_IMAGE, flags)
		update_repeat()
	
	if is_debug_running() and has_meta("done_set"):
		push_warning("Atlas or region are invalid, used fallback image.")
	
	emit_changed()
