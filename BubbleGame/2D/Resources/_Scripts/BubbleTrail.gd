extends Line2D
class_name BubbleTrail

const IntervalNewPoint = 1.0/30.0
var RemainingTimeBeforeNextPoint: float = IntervalNewPoint
@export var WandPosition: Node2D
@export var TrailHitbox: StaticBody2D;
@export var CharacterMovement: PlayerMovement
@export var CharacterHeight: float = 80

var TopLeftPoint: Vector2;
var BottomRightPoint: Vector2;
var StartingPoint: Vector2;

func _ready() -> void:
	StartingPoint = WandPosition.global_position


func _process(delta: float)-> void:
	if (!CharacterMovement.velocity.is_zero_approx()):
		RemainingTimeBeforeNextPoint -= delta;
		if (RemainingTimeBeforeNextPoint <= 0):
			add_point(WandPosition.global_position)
			RemainingTimeBeforeNextPoint = IntervalNewPoint;
			
			var _dbgGlobalPos = WandPosition.global_position;
			TopLeftPoint.x = min(TopLeftPoint.x, WandPosition.global_position.x)
			TopLeftPoint.y = min(TopLeftPoint.y, WandPosition.global_position.y)
			BottomRightPoint.x = max(BottomRightPoint.x, WandPosition.global_position.x)
			BottomRightPoint.y = max(BottomRightPoint.y, WandPosition.global_position.y)
			
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
				await get_tree().create_timer(0.5).timeout
				NewHitbox.disabled = false
		
func GetWidth() -> float:
	return abs(BottomRightPoint.x - TopLeftPoint.x)
	
func GetHeight() -> float:
	return abs(BottomRightPoint.y - TopLeftPoint.y) + CharacterHeight

func GetMiddlePosition() -> Vector2:
	return Vector2(TopLeftPoint.x + (GetWidth() / 2.0), TopLeftPoint.y + (GetHeight() /2.0))
