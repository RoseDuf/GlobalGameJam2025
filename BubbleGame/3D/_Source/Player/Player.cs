using BubbleGame.Common.SceneManagement;
using Godot;

namespace BubbleGame._3D
{
	/// <summary>
	/// This classs keeps track of player info/stats 
	/// and is connected to PlayerMovement and PlayerShooting scripts
	/// </summary>
	public partial class Player : Node3D
	{
		[Export]
		public int health = 1;

		public void Area3D_AreaEntered(Area3D area)
		{
			if (area.Name == "ObstacleArea3D")
			{
				health--;

				if (health <= 0)
				{
					SceneManager.Instance.LoadScene("Results");
				}
			}
		}
	}
}
