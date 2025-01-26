using Godot;

namespace BubbleGame._3D
{
	public partial class Bullet : Node3D
	{
		public Vector3 velocity;

		public override void _Process(double delta)
		{
			base._Process(delta);

			GlobalPosition += velocity * (float)delta;
		}
	}
}
