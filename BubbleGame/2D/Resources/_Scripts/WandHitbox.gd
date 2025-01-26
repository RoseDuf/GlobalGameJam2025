extends Area2D

@export var GameManager: GameManager2D

func _on_body_entered(_body: Node2D) -> void:
	GameManager.PrepareNextPhase()
