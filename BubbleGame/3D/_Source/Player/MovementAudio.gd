extends FmodEventEmitter3D

# Declare member variables here. Examples:
# var a = 2
var event: FmodEvent = null

var previous_position: Vector3
var speed: float = 0.0

# Called when the node enters the scene tree for the first time.
func _ready():
	event = FmodServer.create_event_instance(event_name)
	event.volume = 20
	event.start()
	previous_position = global_transform.origin

func _physics_process(delta):
	# Calculate the speed
	var current_position = global_transform.origin
	var distance = current_position.distance_to(previous_position)
	speed = distance / delta  # Speed = distance / time

 	# Update the previous position
	previous_position = current_position
	event.set_parameter_by_name("Speed", speed)
	

func _exit_tree():
	event.release()
