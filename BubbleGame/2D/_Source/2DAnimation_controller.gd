extends AnimatedSprite2D

@export var GameManager: GameManager2D
# Called when the node enters the scene tree for the first time.
func _ready():
	GameManager.playerDeath.connect(on_player_death)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func on_player_death():
	if GameManager.RemainingSoapTime >= 0:
		play("Hurt")
	else:
		play("Win")
	pass
