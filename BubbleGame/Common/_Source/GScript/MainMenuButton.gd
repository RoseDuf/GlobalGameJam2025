extends BaseButton

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	connect("focus_entered", OnFocusEnter)
	connect("pressed", OnPressed)

func OnFocusEnter() -> void:
	#SFX: Main menu UI Focus button
	pass
	
func OnPressed() -> void:
	#SFX: Main menu UI Press sound
	GlobalAudioManager.PlaySoundOneShot("event:/UI/UIClick")
	if (self.name == "PlayButton"):
		MusicManager.open_game()
		MusicManager.switch_to_2d()
	pass
