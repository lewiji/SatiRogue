tool
extends EditorInspectorPlugin

class_name InspectedObjectGetter

const _inspectedObjects = Dictionary()

static func get_inspected_object():
	return _inspectedObjects['obj']

func can_handle(object):
	print(object.get("name"))
	if (object.get("name") == null or object.get("name") == "Null" or object.get("name") == "RoguelikeMono"):
		return false
	_inspectedObjects['obj'] = object
	print("A real object!")
	print(object)
	return false
