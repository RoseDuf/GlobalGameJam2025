extends TextureProgressBar

@export var GameManager: GameManager2D
@export var HandleOver: HSlider
var soapSound = null
func _ready() -> void:
	soapSound = GlobalAudioManager.PlaySound("event:/2D/BubbleLoop")
	
	HandleOver.max_value = SoapBarManager.MaximumBarValue;
	max_value = SoapBarManager.MaximumBarValue

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	HandleOver.set_value_no_signal(GameManager.RemainingSoapTime)
	set_value_no_signal(GameManager.RemainingSoapTime)
	var SoapRatio = 1 - (GameManager.RemainingSoapTime / max_value)
	soapSound.set_parameter_by_name("SoapBar", SoapRatio)
