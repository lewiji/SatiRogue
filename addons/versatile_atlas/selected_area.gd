tool
extends Panel


func update_rect(rect: Rect2, zoom: float):
	rect_position = rect.position * zoom
	rect_size = rect.size * zoom
