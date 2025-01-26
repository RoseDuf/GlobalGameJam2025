extends Camera2D

@export var Trail: BubbleTrail
@export var DezoomSpeed: float = .995
var TargetZoom: float = 1.0
const CameraMargin = 0.8

func _ready() -> void:
	position = Trail.GetMiddlePosition()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	position = Trail.GetMiddlePosition()
	
	var windowWidth = get_window().size.x;
	var windowHeight = get_window().size.y;
	#var _currentZoom = zoom;
	var viewportAvailableWidth: float = (windowWidth * CameraMargin / zoom.x);
	var viewportAvailableHeight: float = (windowHeight * CameraMargin / zoom.y);
	
	if (Trail.GetWidth() >= viewportAvailableWidth || Trail.GetHeight() >= viewportAvailableHeight):
		var targetZoomX = windowWidth / (Trail.GetWidth() * 1.25);
		var targetZoomY = windowHeight / (Trail.GetHeight() * 1.25);
		TargetZoom = min(targetZoomX, targetZoomY);
	
	zoom = zoom.lerp(Vector2(TargetZoom, TargetZoom), DezoomSpeed) 
	
	#if (zoom.x > TargetZoom):
		#zoom *= DezoomSpeed
		#zoom.x = max(zoom.x, TargetZoom)
		#zoom.y = max(zoom.y, TargetZoom)
