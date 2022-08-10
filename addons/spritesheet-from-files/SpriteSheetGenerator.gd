tool
extends Node

signal process_finished(filepath, err)

func process(data):
	var count = data.files.size()
	var items_per_row : float = min(data.items_per_row, count)
	var w = items_per_row
	var h = ceil(count / items_per_row)
	var spritesheet : Image = _create_image(w, h, data)
	_copy_images_to_spritesheet(spritesheet, w, h, items_per_row, data)
	var err = spritesheet.save_png(data.output_file)
	emit_signal("process_finished", data.output_file, err)
	

func _create_image(w : int, h : int, data):
	var r = Image.new()
	r.create(w * data.frame_width, h * data.frame_height, false, Image.FORMAT_RGBA8)
	return r

func _copy_images_to_spritesheet(spritesheet : Image, w : int, h : int, items_per_row : int, data):
	var rect = Rect2(.0, .0, data.frame_width, data.frame_height)
	var x : int = 0
	var y : int = 0
	for file in data.files:
		var src = file.get_data()
		var pos = Vector2(x * data.frame_width, y * data.frame_height)
		spritesheet.blit_rect(src, rect, pos)
		x += 1
		if x >= items_per_row:
			x = 0
			y += 1
