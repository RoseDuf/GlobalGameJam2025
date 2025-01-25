using Godot;

namespace BubbleGame.Common.Scenes
{
	public partial class MainScene : Node
	{
		[Export]
		private PackedScene _nextScene;

		public override void _Ready()
		{
			CallDeferred(nameof(LoadNextScene));
		}

		private void LoadNextScene()
		{
			if (_nextScene == null)
			{
				GD.PushError($"_nextScene property is null.");
				return;
			}

			GetTree().ChangeSceneToPacked(_nextScene);
		}
	}
}