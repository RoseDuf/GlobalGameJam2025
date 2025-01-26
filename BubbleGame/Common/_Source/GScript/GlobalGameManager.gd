extends Node

const INITIAL_LIVES = 3

var RemainingLives: int = INITIAL_LIVES;
var Score: int = 0

func PrepareForNewGame() -> void:
	RemainingLives = INITIAL_LIVES;
	Score = 0;
