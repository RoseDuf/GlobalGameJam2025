extends FmodEventEmitter3D

@export var GameManager: GameManager2D
# Called when the node enters the scene tree for the first time.
func _ready():
	GameManager.playerDeath.connect(on_playerd_death)
	pass # Replace with function body.


func on_playerd_death():
	self.stop()
