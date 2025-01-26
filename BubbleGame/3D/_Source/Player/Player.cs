using Godot;

namespace BubbleGame._3D
{
	/// <summary>
	/// This classs keeps track of player info/stats 
	/// and is connected to PlayerMovement and PlayerShooting scripts
	/// </summary>
	public partial class Player : Node3D
	{
		public int health = 3;

		public void Area3D_AreaEntered(Area3D area)
		{
			if (area.IsInGroup("obstacle"))
			{
				health--;
			}
		}
	}
}
