extends CharacterBody2D
class_name PlayerMovement

@export var GameManager: GameManager2D
@export var CharacterSpeed: float = 300.0
@export var CharacterAnimations: AnimatedSprite2D;
@export var CharacterWand: Sprite2D;
var CharacterCurrentDirectionAngle: float = Vector2.RIGHT.angle() #To the right by default
var CharacterWantedDirectionAngle: float = CharacterCurrentDirectionAngle;
var rotationElapsed: float = 0
var input_enabled = true
var footstepSounds = null

func _ready() -> void:
	GameManager.playerDeath.connect(on_playerd_death)
	footstepSounds = GlobalAudioManager.PlaySound("event:/2D/Footsteps")
	
func get_input(delta: float) -> void:
	var inputDir = Input.get_vector("move_left","move_right","move_up","move_down")
	if (!inputDir.is_zero_approx()):
		CharacterCurrentDirectionAngle = velocity.angle()
		CharacterWantedDirectionAngle = inputDir.angle()
		rotationElapsed = 0.1 #Still being able to turn when holding
	
	var angleDirection = lerp_angle(CharacterCurrentDirectionAngle, CharacterWantedDirectionAngle, rotationElapsed)
	velocity = Vector2.from_angle(angleDirection) * CharacterSpeed;	
	rotationElapsed = min(rotationElapsed + delta, 1)	

func _physics_process(delta: float) -> void:
	if input_enabled:
		get_input(delta)	
		if (move_and_slide()):
			GameManager.PrepareNextPhase()

func _process(_delta: float) -> void:
	if input_enabled:
		CharacterAnimations.flip_h = velocity.x < -0.01
		CharacterWand.flip_h = CharacterAnimations.flip_h

func on_playerd_death():
	footstepSounds.stop(FmodServer.FMOD_STUDIO_STOP_ALLOWFADEOUT)
	footstepSounds.release()
	input_enabled = false
