extends FmodEventEmitter2D


# Called when the node enters the scene tree for the first time.
#func _ready() -> void:
#	event = FmodServer.create_event_instance(event_name)
	#event.volume = 20
#	event.start()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func open_menu():
	FmodServer.set_global_parameter_by_name("IsInGame", 0)
	
func open_game():
	FmodServer.set_global_parameter_by_name("IsInGame", 1)
	
func switch_to_3d():
	FmodServer.set_global_parameter_by_name("GameMode", 3)
	
func switch_to_2d():
	FmodServer.set_global_parameter_by_name("GameMode", 2)
	
