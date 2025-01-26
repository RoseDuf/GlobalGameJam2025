extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	open_menu()
	pass # Replace with function body.

func PlaySound(path):
	var event: FmodEvent = null
	event = FmodServer.create_event_instance(path)
	event.start()
	return event
	
func PlaySoundOneShot(path):
	FmodServer.play_one_shot(path)
	pass
	
func open_menu():
	FmodServer.set_global_parameter_by_name("IsInGame", 0)
	
func open_game():
	FmodServer.set_global_parameter_by_name("IsInGame", 1)
	
func switch_to_3d():
	FmodServer.set_global_parameter_by_name("GameMode", 3)
	
func switch_to_2d():
	FmodServer.set_global_parameter_by_name("GameMode", 2)
