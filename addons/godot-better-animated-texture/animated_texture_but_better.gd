tool
extends AnimatedTexture


const ALLOWED_FILE_EXTENSIONS: PoolStringArray = PoolStringArray([
	"png", "jpg", "jpeg", "gif", "tiff", "tif"
])

export(String, DIR) var sprites_dir: String setget set_sprites_dir


func set_sprites_dir(new_sprites_dir: String):
	sprites_dir = new_sprites_dir

	frames = 1

	if not sprites_dir:
		set_frame_texture(0, null)

	var dir: Directory = Directory.new()
	if dir.open(sprites_dir) == OK:
		dir.list_dir_begin()

		var file_name: String = dir.get_next()
		var texture_id: int = -1
		while file_name:
			var name_extension_split: PoolStringArray = (
				file_name.rsplit(".", true, 1)
			)

			if (name_extension_split[1] in ALLOWED_FILE_EXTENSIONS):
				var texture_id_split: Array = name_extension_split[0].rsplit(
					"_", true, 1
				)

				if (texture_id_split.size() >= 2 and
						texture_id_split[1].is_valid_integer()):
					texture_id = int(texture_id_split[1])
				else:
					texture_id += 1

				if texture_id + 1 > frames:
						frames = texture_id + 1

				set_frame_texture(
					texture_id,
					load("%s/%s" % [sprites_dir, file_name])
				)

			file_name = dir.get_next()

		dir.list_dir_end()
