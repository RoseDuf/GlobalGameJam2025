extends TextureProgressBar

@export var GameManager: GameManager2D
@export var HandleOver: HSlider
var soapSound = null
var stop_soap_update = false
func _ready() -> void:
	soapSound = GlobalAudioManager.PlaySound("event:/2D/BubbleLoop")
	
	HandleOver.max_value = SoapBarManager.MaximumBarValue;
	max_value = SoapBarManager.MaximumBarValue
	GameManager.playerDeath.connect(playerDeath)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	if !stop_soap_update:
		HandleOver.set_value_no_signal(GameManager.RemainingSoapTime)
		set_value_no_signal(GameManager.RemainingSoapTime)
		var SoapRatio = 1 - (GameManager.RemainingSoapTime / max_value)
		soapSound.set_parameter_by_name("SoapBar", SoapRatio)

func playerDeath():
	stop_soap_update = true
	soapSound.set_parameter_by_name("SoapBar", 1.1)
