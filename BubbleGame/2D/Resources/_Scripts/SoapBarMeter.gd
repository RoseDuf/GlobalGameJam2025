extends TextureProgressBar

@export var GameManager: GameManager2D
@export var HandleOver: HSlider

func _ready() -> void:
	HandleOver.max_value = SoapBarManager.MaximumBarValue;
	max_value = SoapBarManager.MaximumBarValue

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	HandleOver.set_value_no_signal(GameManager.RemainingSoapTime)
	set_value_no_signal(GameManager.RemainingSoapTime)
