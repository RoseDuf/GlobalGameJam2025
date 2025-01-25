@tool
extends Line2D

const IntervalNewPoint = 1.0/30.0
var RemainingTimeBeforeNextPoint: float = IntervalNewPoint
@export var WandPosition: Node2D
@export var TrailHitbox: StaticBody2D;
@export var CharacterMovement: PlayerMovement


func _process(delta: float)-> void:
	if (!CharacterMovement.velocity.is_zero_approx()):
		RemainingTimeBeforeNextPoint -= delta;
		if (RemainingTimeBeforeNextPoint <= 0):
			add_point(WandPosition.global_position)
			RemainingTimeBeforeNextPoint = IntervalNewPoint;
			
			#var hitboxesSize = TrailHitbox.get_child_count();
			#if (hitboxesSize > 1):
				#TrailHitbox.get_child(hitboxesSize - 1).disabled = false
				
			
			var pointsSize = points.size()
			if (pointsSize > 1):
				var NewHitbox: CollisionShape2D = CollisionShape2D.new()
				TrailHitbox.add_child(NewHitbox);
				var HitboxShape = SegmentShape2D.new();
				HitboxShape.a = points[pointsSize - 2];
				HitboxShape.b = points[pointsSize - 1];
				NewHitbox.shape = HitboxShape
				NewHitbox.disabled = true
				await get_tree().create_timer(1.0).timeout
				NewHitbox.disabled = false
