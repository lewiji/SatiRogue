extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	var texNames = ["albedo", "normal", "occlusion", "parallax", "specular"]
	var textures = []
	for texName in texNames:
		textures.append(load("res://assets/elthen/elthen_" + texName + ".png"))
		
	var imgArray = []
	for index in range(0, textures.size()):
		var texture = textures[index]
		var image = texture.get_image()
		if (image.is_compressed()):
			image.decompress()
		
		image.convert(Image.FORMAT_RGBA8)
		imgArray.append(image)
	
	var texArray = Texture2DArray.new()
	var result = texArray.create_from_images(imgArray)
	var texArrPath = "res://assets/elthen/elthen_tex_array.tres"

	print("Saving to " + texArrPath)
	ResourceSaver.save(texArray, texArrPath)
