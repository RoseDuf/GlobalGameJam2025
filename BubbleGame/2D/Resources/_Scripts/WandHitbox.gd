extends Area2D

@export var GameManager: GameManager2D
@export var Character: AnimatedSprite2D
@export var WandFront: Sprite2D

func _process(_delta: float) -> void:
	pass;
	#WandFront.global_position = Character.global_position;

func _on_body_entered(body: Node2D) -> void:
	
	if (body is CollisionObject2D):
		var collision:CollisionObject2D = body as CollisionObject2D
		
		if (collision.get_collision_layer_value(3)):
			var bugCollided = body.get_parent().get_parent() as BugCollectible
			GameManager.CollectBug(bugCollided.IncreaseScore())
			bugCollided.queue_free()
			return; #Return now so it doesn't go to the next phase
	
	GameManager.PrepareNextPhase()
