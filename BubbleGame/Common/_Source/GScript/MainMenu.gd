extends Node


func OnPlayButtonPressed() -> void:
	GlobalGameManager.PrepareForNewGame();
	
	#Not the best but works
	get_tree().change_scene_to_file("res://BubbleGame/2D/Resources/_Scenes/Bubble2D-MainWorld.tscn")


func OnQuitButtonPressed() -> void:
	get_tree().quit()
