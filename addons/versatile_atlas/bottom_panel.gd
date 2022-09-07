tool
extends Control


signal close
signal request_changes(rect)
signal zoom_changed(new_zoom)

const ZOOM_MIN = 0.0625
const ZOOM_MAX = 16.0

var atlas: Texture setget set_atlas
# 2 == 2x zoom IN
var zoom: float = 1.0 setget set_zoom
var first_time_open: bool = true

onready var HasTex = $HasTex
onready var BelowTex = $HasTex/Below/Inside/TextureRect
onready var Below = $HasTex/Below
onready var Inside = $HasTex/Below/Inside
onready var ScrollX = $HasTex/Below/HScrollBar
onready var ScrollY = $HasTex/Below/VScrollBar
onready var DrawGrid = $HasTex/Below/DrawGrid


func _ready():
	if not is_in_editor(): return
	set_scroll_bars()
	_on_general_scroll()
	var size = Below.rect_size
	var tex_size = atlas.get_size() if atlas else Vector2.ZERO
	ScrollX.value = size.x - tex_size.x
	ScrollY.value = size.y - tex_size.y


func set_used_rect(rect: Rect2):
	Inside.start = rect.position
	Inside.end = rect.end
	Inside.update_rect()


func set_scroll_bars():
	var size = Below.rect_size
	var tex_size = atlas.get_size() if atlas else Vector2.ZERO
	ScrollX.page = size.x / zoom
	ScrollY.page = size.y / zoom
	ScrollX.max_value = 2 * size.x + tex_size.x
	ScrollY.max_value = 2 * size.y + tex_size.y
	Inside.base_pos = size


func _on_BottomPanel_resized():
	if not is_in_editor(): return
	if not ScrollX: return
	set_scroll_bars()
	_on_general_scroll()


func set_zoom(val):
	zoom = clamp(val, ZOOM_MIN, ZOOM_MAX)
	set_scroll_bars()
	_on_general_scroll()
	emit_signal("zoom_changed", zoom)


func _on_BottomPanel_zoom_changed(new_zoom):
	BelowTex.rect_scale = Vector2.ONE * new_zoom


func _on_general_scroll():
	var cam_pos = Vector2(ScrollX.value, ScrollY.value)
	Inside.move_to_pos(-cam_pos)
	DrawGrid.draw_grid(Inside.snap_vec, Inside.snap_offset, cam_pos, zoom, Inside.base_pos)


func set_atlas(val):
	atlas = val
	BelowTex.texture = val
	set_scroll_bars()
	_on_general_scroll()


func opened_first_time():
	ScrollX.value = ScrollX.max_value * 0.5
	ScrollY.value = ScrollY.max_value * 0.5


func _on_BottomPanel_visibility_changed():
	if not is_in_editor(): return
	if not ScrollX: return
#	print("DONE")
	set_scroll_bars()
	_on_general_scroll()
	get_tree().connect("idle_frame", self, "deferred_call", [], CONNECT_ONESHOT)


func deferred_call():
	if first_time_open:
		opened_first_time()
		first_time_open = false
	set_scroll_bars()
	_on_general_scroll()


func is_in_editor():
	return get_viewport().get_parent() == null


func _on_Button_pressed():
	emit_signal("close")


func _on_Apply_pressed():
	emit_signal("request_changes", Inside.get_selected_rect())


func move_relative(rel: Vector2):
	ScrollX.value -= rel.x
	ScrollY.value -= rel.y
	_on_general_scroll()

################################################################################

func _on_StepX_value_changed(value):
	Inside.snap_vec.x = value
	_on_general_scroll()


func _on_StepY_value_changed(value):
	Inside.snap_vec.y = value
	_on_general_scroll()


func _on_OffsetX_value_changed(value):
	Inside.snap_offset.x = value
	_on_general_scroll()


func _on_OffsetY_value_changed(value):
	Inside.snap_offset.y = value
	_on_general_scroll()

################################################################################

func _on_Less_pressed():
	set_zoom(zoom * 0.707109) # 1 / sqrt(2)


func _on_More_pressed():
	set_zoom(zoom * 1.41421) # sqrt(2)


func _on_One_pressed():
	set_zoom(1)
