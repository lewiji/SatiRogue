tool
extends Control


const GRID_THERESHOLD = 4
# Not recognized as constant expression???
var GRID_COLOR = ColorN("gray", 0.5)

var tile_size: Vector2 = Vector2.ONE
var tile_offset: Vector2 = Vector2.ZERO
var cam_pos: Vector2 = Vector2.ZERO
var cam_zoom: float = 1.0
var base_pos: Vector2 = Vector2.ZERO


func draw_grid(size: Vector2, offset: Vector2, pos: Vector2, zoom: float, base: Vector2):
	tile_size = size
	tile_offset = offset
	cam_pos = pos
	cam_zoom = zoom
	base_pos = base
	update()


func _draw():
#	print((cam_pos.x - tile_offset.x) / tile_size.x)
	var i = 0
	
	# A bunch of math and equation
	if tile_size.x >= GRID_THERESHOLD:
		i = floor((cam_pos.x * cam_zoom - tile_offset.x * cam_zoom - base_pos.x * cam_zoom) / (tile_size.x * cam_zoom))
		while true:
			var pos = (tile_offset.x + i * tile_size.x - cam_pos.x + base_pos.x) * cam_zoom
			draw_h_line(pos)
			i += 1
			if pos >= rect_size.x: break
	
	if tile_size.y >= GRID_THERESHOLD:
		i = floor((cam_pos.y * cam_zoom - tile_offset.y * cam_zoom - base_pos.y * cam_zoom) / (tile_size.y * cam_zoom))
		while true:
			var pos = (tile_offset.y + i * tile_size.y - cam_pos.y + base_pos.y) * cam_zoom
			draw_v_line(pos)
			i += 1
			if pos >= rect_size.y: break


func draw_h_line(x: float):
	draw_line(Vector2(x, 0), Vector2(x, rect_size.y), GRID_COLOR)


func draw_v_line(y: float):
	draw_line(Vector2(0, y), Vector2(rect_size.x, y), GRID_COLOR)
