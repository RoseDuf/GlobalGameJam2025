extends CharacterBody2D
class_name PlayerMovement

@export var GameManager: GameManager2D
@export var CharacterSpeed: float = 300.0
var PreviousDirection: Vector2 = Vector2(0,0)
var CurrentAcceleration: float = 0

func get_input(delta: float) -> void:
	var inputDir = Input.get_vector("move_left","move_right","move_up","move_down")
	if (inputDir.is_zero_approx()):
		CurrentAcceleration = maxf(0, CurrentAcceleration - delta)
	else:
		CurrentAcceleration = minf(1, CurrentAcceleration + delta)
	
	PreviousDirection = PreviousDirection.lerp(inputDir, .33);
	velocity = PreviousDirection * (CharacterSpeed * CurrentAcceleration);
	

func _physics_process(delta: float) -> void:
	get_input(delta)	
	if (move_and_slide()):
		GameManager.PrepareNextPhase()
