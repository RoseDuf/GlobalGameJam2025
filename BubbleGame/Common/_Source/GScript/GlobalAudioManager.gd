extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func PlaySound(path):
	var event: FmodEvent = null
	event = FmodServer.create_event_instance(path)
	event.start()
	return event
	
func PlaySoundOneShot(path):
	FmodServer.play_one_shot(path)
	pass
	
