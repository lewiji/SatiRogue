tool
extends HBoxContainer

const DEBUG_VERBOSE = false

var editorb:EditorInterface #The EditorInterface sent to us from the plugin activation
var _activated:bool


onready var editor_node = get_node("/root/EditorNode")
var editor_canvas #= find_viewport(editor_node,0, "CanvasItemEditor")
var viewport:Viewport  #= find_viewport(editor_canvas,0,"Viewport")
#var zoomControl

var font 

var lastpos:Vector2 = Vector2.ZERO
var lastzoom:float = 1.0
var pad_amt = 1
	
var zLevels = [0.25, 0.5, 1.0, 2.0, 3.0, 4.0,
			6.0, 8.0, 12.0, 16.0
			]
var padLevels = [0,0,0,0,1,1,2,2,2,3]

#Used to ready the addon once the editor interface has been acquired from host
func activate():
	self._activated = true
	#listKids(viewport,0)
#	font = editorb.get_base_control().theme.default_font

	font = self.get_font("font")  #GDScript 3.0

#	self.font = self.get_theme_font("font")  #GDScript 4.0

	refresh(self)
	set_physics_process(true)
	
	$ZoomLevel.clear()

	$ZoomLevel.add_item("??? %", 0xFF)  #Custom/Current
#	$ZoomLevel.set_item_disabled($ZoomLevel.get_item_index(0xFF), true)  #Custom/Current
	$ZoomLevel.add_separator()

	for i in len(zLevels):
		$ZoomLevel.add_item(str(int(zLevels[i]*100)) + "%", i)


#in case something goes horribly wrong!
func refresh(from_whom):
	if DEBUG_VERBOSE: print ("Zoomy refresh!")
	
	if !_activated:
		if DEBUG_VERBOSE: print("Zoomy was reinitialized.  Please restart")
		activate()
		return
	
	#Let's find the canvas viewport, LIKE A WRECKING BALL
	self.editor_canvas = find_viewport($'/root/EditorNode', 0 ,"CanvasItemEditor")

#	#Godot 3.x
	self.viewport = find_viewport(editor_canvas, 0, "Viewport", DEBUG_VERBOSE) # Returns null if not found

#	#Godot 4.0
#	var vp = editor_canvas.get_viewport()
#	self.viewport = vp


#===================================
#	#Really really stupid considering the vagueness but in Godot 4 the viewport
#	#Can't report size, so instead of find CanvasItemEditor.viewport_scrollable,
#	#We search for its type, which is Control, and it appears to be the only one
#	#Of this type as of 804ee245a.
#	self.sv = find_viewport($'/root/EditorNode', 0, "Control", DEBUG_VERBOSE)
#	print(self.sv)
	

#	self.zoomControl = find_viewport($'/root/EditorNode', 0, "Button", DEBUG_VERBOSE, "Zoom Reset")
#	self.zoomControl = find_viewport(editor_canvas, 0, "Control", DEBUG_VERBOSE, "Zoom Reset")

#Done every frame. I wonder if there's a way to only do it on the proper signals?
func _physics_process(delta):	
	if !OS.is_window_focused():  return
	if !self._activated:
#		print("Activation no longer true, please restart zoomy")
		return
		#Don't remember if this is necessary.  Might be here for future use
		#if EditorInterface can expose the 2d viewport without nasty hacks
#		var root=editorb.get_base_control()

	if self.viewport !=null: #and self.zoomControl !=null:
		var zoom = viewport.get_final_transform().get_scale().x  #Godot 3.3.2 (Might support the below later)
#		var zoom = editor_canvas.get_state()["zoom"]  #Godot 4.0
		var mpos = viewport.get_mouse_position()
#		prints(zoom,mpos)
					
#		$ZSnap/Z.text = "%1.1f%%" % zoom
		$posSnap/pos.text = "(%s, %s)" % [str(mpos.x).pad_decimals(pad_amt), str(mpos.y).pad_decimals(pad_amt)]
		
		$ZoomLevel.text ="%1.1f%%" % (zoom*100)


		#Only redraw when you gotta.
		#the position string changes a LOT so we need to make sure
		#that it only resizes when it needs more room to reduce
		#the appearance of the string 'jumping around' a lot
		if not lastpos == mpos:
			var posWidth = font.get_string_size("(,)").x 
			posWidth = (max(6,len($posSnap/pos.text )-3)) * 9 + posWidth -2
			$posSnap.rect_min_size.x = posWidth
			$posSnap/pos.rect_size.x = posWidth
			
		if not lastzoom == zoom:
			var width = font.get_string_size(".%").x
			width = (len($ZSnap/Z.text)-2) * 9 + width + 4
			$ZSnap.rect_min_size.x = width
			$ZSnap/Z.rect_size.x = width
			
			#Find closest value in our array to zoom level and select it
			var closest = zLevels.find(zoom)
			if closest >= 0:
				$ZoomLevel.select($ZoomLevel.get_item_index(closest))
			else:
				$ZoomLevel.select(0) 

			closest = zLevels.bsearch(zoom)
			pad_amt = padLevels[min(padLevels.size()-1, closest)]

			pass

		
		lastpos = mpos
		lastzoom = zoom


#func find_zoom_control(node, recursive_level, nodes=[]):
#	if node == null:
#		print("null bad! %s," % [recursive_level])
#		return null
#	if node is Control and node.hint_tooltip.find("Zoom Reset") >= 0:
#		if DEBUG_VERBOSE: print( "Zoom control found." )
#		nodes.append(node)
##		return node
#
#	else:
#		recursive_level += 1
#		if recursive_level > 24:
#			return null
#		for child in node.get_children():
##			if DEBUG_VERBOSE: print(repeat_str(recursive_level) + "Child: %s (%s)" % [child.name, child.get_class()])
#			var result = find_zoom_control(child, recursive_level, nodes)
#			if result != null:
#				return result
	

#FIND THE EDITOR VIEWPORT
func find_viewport(node, recursive_level, className, debugPrint=false, searchTip:String=""):
#	print("SearchTip for %s: " % className, searchTip)
	if node == null:
		print("null bad! %s, %s" % [recursive_level, className])
		return null
	if node.get_class() == className and node is Control and node.hint_tooltip.find(searchTip) >= 0:
		if DEBUG_VERBOSE: print( "%s Found by name." % className)
		return node
	elif node.get_class() == className and searchTip=="":
		if DEBUG_VERBOSE: print( "%s Found.." % className)
		return node
		
	else:
		recursive_level += 1
		if recursive_level > 15:
			return null
		for child in node.get_children():
			if debugPrint == true: print(repeat_str(recursive_level) + "Child: %s (%s)" % [child.name, child.get_class()])
			var result = find_viewport(child, recursive_level, className, false, searchTip)
			if result != null:
				return result

#DEBUG ONLY, this is used to inspect the root tree so we can see what's in there.
func listKids(node, recursive_level):
	recursive_level +=1
	if recursive_level >15:
		return null
	for child in node.get_children():
		var tip = child.hint_tooltip if child is Control else ""
		print(repeat_str(recursive_level) + "Child: %s (%s)  %s" % [child.name, child.get_class(), tip])
		var result = listKids(child,recursive_level)
		if result != null:
			return result
			
#This function is dumb.  It only repeats spaces because reasons
func repeat_str(length):
	return " ".repeat(length)
#	return ('%' + String(length*4) + 'c') % 33

#Change zoom level because the OptionButton said so
func _on_ZoomLevel_item_selected(ID):
	if viewport == null or ID < 2:  return
	
#	var zoom = editor_canvas.get_state()["zoom"]

	zoom_on_pos(zLevels[ID-2], viewport.size/2.0)


func zoom_on_pos(p_zoom:float, p_position):
	var prev_zoom:float = viewport.global_canvas_transform.get_scale().x #Godot 3.x
#	var prev_zoom = editor_canvas.get_state()["zoom"]  #Godot 4.0

	var offset = viewport.global_canvas_transform.origin / prev_zoom

	editor_canvas.set_state({"zoom": p_zoom})
#	yield(get_tree(), "idle_frame")
	editor_canvas.set_state({"ofs": -offset+ (p_position/prev_zoom - p_position / p_zoom)})


func _on_ZoomLevel_pressed():
	$ZoomLevel.set_item_text(0, $ZoomLevel.text)
