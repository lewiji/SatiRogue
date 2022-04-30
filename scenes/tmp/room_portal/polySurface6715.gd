extends MeshInstance

func _ready():
	print(get_transformed_aabb().position)
	print(get_transformed_aabb().size)
	print(get_transformed_aabb().end)
