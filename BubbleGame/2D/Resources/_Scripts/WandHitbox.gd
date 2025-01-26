extends Area2D

@export var GameManager: GameManager2D

func _on_body_entered(body: Node2D) -> void:
	
	if (body is CollisionObject2D):
		var collision:CollisionObject2D = body as CollisionObject2D
		
		if (collision.get_collision_layer_value(3)):
			#var bugCollided = body.get_parent().get_parent() as BugCollectible
			#GameManager.CollectBug(bugCollided.IncreaseScore())
			#bugCollided.queue_free()
			return; #Return now so it doesn't go to the next phase
	
	GameManager.PrepareNextPhase()
