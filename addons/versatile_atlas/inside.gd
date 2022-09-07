tool
extends Control


const SCROLLBAR_SIZE = 12
const SCROLL_SENS = 35

var selecting = false
var start: Vector2 = Vector2.ZERO
var end: Vector2 = Vector2.ZERO
var snap_vec: Vector2 = Vector2.ONE
var snap_offset: Vector2 = Vector2.ZERO
var base_pos: Vector2 = Vector2(256, 256)

onready var SelectedArea = $SelectedArea
onready var TexRect = $TextureRect
onready var Parent = $".."
onready var ScnRoot = $"../../.."
onready var Zoom = $"../Zoom"


func _ready():
	set_process(is_in_editor())
	set_process_input(is_in_editor())
	if not is_in_editor(): return
	move_to_pos(Vector2.ZERO)


func _process(_delta):
	if selecting:
		end = snap(get_local_mouse_position())
		update_rect()
#		printt(start, end)


func update_rect():
	SelectedArea.update_rect(get_selected_rect(), ScnRoot.zoom)


func _on_BottomPanel_zoom_changed(new_zoom):
	update_rect()


func move_to_pos(vec: Vector2):
	rect_position = (vec + base_pos) * ScnRoot.zoom


func snap(vec: Vector2) -> Vector2:
	return (vec / ScnRoot.zoom - snap_offset).snapped(snap_vec) + snap_offset


func get_selected_rect():
	return Rect2(start, end - start).abs()


func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT:
			if event.is_pressed():
				if not is_in_rect(get_global_mouse_position()):
					return
				if not is_visible_in_tree():
					return
	#			print(is_visible_in_tree())
				selecting = true
				start = snap(get_local_mouse_position())
			else:
				selecting = false
		
		if event.button_index == BUTTON_WHEEL_UP:
			if event.is_pressed():
				if not is_in_rect(get_global_mouse_position()):
					return
				if not is_visible_in_tree():
					return
				ScnRoot.move_relative(Vector2(0, SCROLL_SENS))
		
		if event.button_index == BUTTON_WHEEL_DOWN:
			if event.is_pressed():
				if not is_in_rect(get_global_mouse_position()):
					return
				if not is_visible_in_tree():
					return
				ScnRoot.move_relative(Vector2(0, -SCROLL_SENS))
	
	if event is InputEventMouseMotion:
		if event.button_mask & BUTTON_MASK_RIGHT:
			if not is_in_rect(get_global_mouse_position()):
				return
			if not is_visible_in_tree():
				return
			ScnRoot.move_relative(event.relative)


func is_in_rect(point: Vector2) -> bool:
	if not is_visible_in_tree(): return false
	
	if Zoom.get_global_rect().has_point(point):
		return false
	
	var rect = Parent.get_global_rect()
	# Have to do this or else it would do it when scrolling with bar
	rect.size -= Vector2.ONE * SCROLLBAR_SIZE
	return rect.has_point(point)


func is_in_editor():
	return get_viewport().get_parent() == null
