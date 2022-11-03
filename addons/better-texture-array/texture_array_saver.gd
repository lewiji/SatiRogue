tool
extends ResourceFormatSaver
class_name TextureArraySaver

func get_recognized_extensions(res: Resource) -> PoolStringArray:
	return PoolStringArray(["texarr", "tex3d"]) if recognize(res) else PoolStringArray()

func recognize(res: Resource) -> bool:
	return res is TextureArray or res is Texture3D

func save(path: String, res: Resource, flags: int) -> int:
	var imp = preload("res://addons/better-texture-array/texture_array_builder.gd").new()
	imp.is_3d = path.get_extension() == "tex3d"
	res.take_over_path(path)
	return imp._save_tex(res.data.layers, path, imp.Compress.LOSSLESS, -1, res.flags)
