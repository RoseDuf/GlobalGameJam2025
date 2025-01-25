using Godot;

using BubbleGame.Common.SceneManagement;

namespace BubbleGame.Common
{
	public partial class MainMenuView : Control
	{
		private void PlayButton_Pressed()
		{
			SceneManager.Instance.LoadScene("Bubble2D");
		}

		private void _2DButton_Pressed()
		{
			SceneManager.Instance.LoadScene("Bubble2D");
		}

		private void _3DButton_Pressed()
		{
			SceneManager.Instance.LoadScene("Bubble3D");
		}

		private void CreditsButton_Pressed()
		{
			SceneManager.Instance.LoadScene("Credits");
		}

		private void QuitButton_Pressed()
		{
			GetTree().Quit();
		}
	}
}