extends Button

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	visible = OS.is_debug_build()
