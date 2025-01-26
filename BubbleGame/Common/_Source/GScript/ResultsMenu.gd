extends Control

@export var ScoreText: RichTextLabel

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ScoreText.text = str(GlobalGameManager.Score);


func OnPlayButtonPressed() -> void:
	GlobalGameManager.PrepareForNewGame();
	
	#Not the best but works
	get_tree().change_scene_to_file("res://BubbleGame/2D/Resources/_Scenes/Bubble2D-MainWorld.tscn")


func OnQuitButtonPressed() -> void:
	get_tree().quit()
