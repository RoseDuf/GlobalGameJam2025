extends FmodEventEmitter3D

# Declare member variables here. Examples:
# var a = 2
var event: FmodEvent = null

# Called when the node enters the scene tree for the first time.
func _ready():
	event = FmodServer.create_event_instance(event_guid)
	event.set_3d_attributes(self.global_transform)
	event.start()

func _exit_tree():
	event.release()
