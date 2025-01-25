@tool
extends CharacterBody2D

@export var CharacterSpeed: float = 300.0
var PreviousDirection: Vector2 = Vector2(0,0)
var currentAcceleration: float = 0

func get_input(delta: float) -> void:
	var inputDir = Input.get_vector("move_left","move_right","move_up","move_down")
	if (inputDir.is_zero_approx()):
		currentAcceleration = maxf(0, currentAcceleration - delta)
	else:
		currentAcceleration = minf(1, currentAcceleration + delta)
	
	PreviousDirection = PreviousDirection.lerp(inputDir, .33	);
	velocity = PreviousDirection * (CharacterSpeed * currentAcceleration);	
	

func _physics_process(delta: float) -> void:
	get_input(delta)	
	if (move_and_slide()):
		assert(false, "OH NOEZ GAME IS OVER")
