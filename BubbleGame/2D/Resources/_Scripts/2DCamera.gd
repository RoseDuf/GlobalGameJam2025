extends Camera2D

@export var Trail: BubbleTrail
var DezoomSpeed: float = 1
var TargetZoom: float = 1.0
const CameraMargin = .75

func _ready() -> void:
	position = Trail.GetMiddlePosition()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
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
		DezoomSpeed = 0.1
	
	zoom = zoom.lerp(Vector2(TargetZoom, TargetZoom), DezoomSpeed) 
	DezoomSpeed = min(DezoomSpeed + delta, 1)
	
	
	#if (zoom.x > TargetZoom):
		#zoom *= DezoomSpeed
		#zoom.x = max(zoom.x, TargetZoom)
		#zoom.y = max(zoom.y, TargetZoom)
