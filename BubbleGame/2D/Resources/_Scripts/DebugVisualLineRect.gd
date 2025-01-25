extends Sprite2D

@export var Trail: BubbleTrail

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	global_position = Trail.GetMiddlePosition()
