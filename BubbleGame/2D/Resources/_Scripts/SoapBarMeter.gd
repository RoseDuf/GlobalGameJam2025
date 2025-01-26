extends TextureProgressBar

@export var GameManager: GameManager2D

func _ready() -> void:
	max_value = SoapBarManager.MaximumBarValue

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	set_value_no_signal(GameManager.RemainingSoapTime)
