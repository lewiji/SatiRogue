tool
extends Spatial

onready var animation_player: AnimationPlayer = get_node("AnimationPlayer")

func _ready() -> void:
	animation_player.play("spin")
