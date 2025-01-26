extends Node

enum BUGS {Bee, Butterfly, Mosquito}

const INITIAL_LIVES = 3

var RemainingLives: int = INITIAL_LIVES;
var Score: int = 0

func PrepareForNewGame() -> void:
	RemainingLives = INITIAL_LIVES;
	Score = 0;

func _input(event: InputEvent) -> void:
	if (event.is_action_pressed("escape") && OS.is_debug_build()):
		call_deferred("LoadMainMenu")
	
func LoadMainMenu() -> void:
	get_tree().change_scene_to_file("res://BubbleGame/Common/Resources/_Scenes/Welcome/WelcomeScene.tscn")
