extends Node
class_name GameManager2D	

var RemainingSoapTime: float = SoapBarManager.INITIAL_MAX_VALUE


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	RemainingSoapTime = SoapBarManager.MaximumBarValue;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	RemainingSoapTime -= delta;
	
	if (RemainingSoapTime <= 0):
		PrepareNextPhase()

func PrepareNextPhase() -> void:
	assert(false, "NOT TEH GAEM OVERZ")
	
