extends Node
class_name GameManager2D	

var RemainingSoapTime: float = SoapBarManager.INITIAL_MAX_VALUE
var CollectedBugsList: Array;

const SectionName = "CachedData"
signal playerDeath
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	GlobalAudioManager.open_game()
	RemainingSoapTime = SoapBarManager.MaximumBarValue;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	RemainingSoapTime -= delta;
	
	if (RemainingSoapTime <= 0):
		PrepareNextPhase()

func CollectBug(bug: GlobalGameManager.BUGS) -> void:
	CollectBugTime(SoapBarManager.MaximumBarValue - RemainingSoapTime, bug);

func CollectBugTime(time: float, bug: GlobalGameManager.BUGS) -> void:
	CollectedBugsList.append([time, bug]	);

func PrepareNextPhase() -> void:
	
	#Saving progress for part 2
	var progressFile : ConfigFile = ConfigFile.new();
	
	# Checking if file exists, so we clear its content for repeated playthroughs
	var error = progressFile.load("user://levelcache.cfg")
	if (error == OK):
		progressFile.clear()
		
	progressFile.set_value(SectionName, "TimeElapsed", SoapBarManager.MaximumBarValue - RemainingSoapTime)
	progressFile.set_value(SectionName, "TimeStamps", CollectedBugsList)
	
	progressFile.save("user://levelcache.cfg")
	
	call_deferred("LoadNextLevel")
	
func LoadNextLevel() -> void:
	playerDeath.emit()
	var timer:SceneTreeTimer = get_tree().create_timer(2.5)
	timer.timeout.connect(_on_timer_timeout)  

	
func _on_timer_timeout():
	GlobalAudioManager.switch_to_3d()
	get_tree().change_scene_to_file("res://BubbleGame/3D/Resources/_Scenes/Bubble3D.tscn")
