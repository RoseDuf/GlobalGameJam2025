extends Node
class_name GameManager2D	

var RemainingSoapTime: float = SoapBarManager.INITIAL_MAX_VALUE
var CollectedBugsList: Dictionary;

const SectionName = "CachedData"

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	RemainingSoapTime = SoapBarManager.MaximumBarValue;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	RemainingSoapTime -= delta;
	
	if (RemainingSoapTime <= 0):
		PrepareNextPhase()

func CollectBug(time: float, bug: GlobalGameManager.BUGS) -> void:
	CollectedBugsList.get_or_add(time, bug);

func PrepareNextPhase() -> void:
	
	#TODO: Remove made-up data when collecting works
	CollectBug(3.0, GlobalGameManager.BUGS.Bee);
	CollectBug(5.01465, GlobalGameManager.BUGS.Mosquito);
	CollectBug(8.0, GlobalGameManager.BUGS.Bee);
	CollectBug(10.0, GlobalGameManager.BUGS.Butterfly);
	
	#Saving progress for part 2
	var progressFile : ConfigFile = ConfigFile.new();
	
	# Checking if file exists, so we clear its content for repeated playthroughs
	var error = progressFile.load("user://levelcache.cfg")
	if (error == OK):
		progressFile.clear()
		
	for key in CollectedBugsList:
		progressFile.set_value(SectionName, str(key), CollectedBugsList[key])
	
	progressFile.save("user://levelcache.cfg")
	
	call_deferred("LoadNextLevel")
	
func LoadNextLevel() -> void:
	get_tree().change_scene_to_file("res://BubbleGame/3D/Resources/_Scenes/Bubble3D.tscn")
