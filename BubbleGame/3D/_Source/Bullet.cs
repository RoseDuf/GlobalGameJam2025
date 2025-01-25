using Godot;
using System;

namespace BubbleGame._3D
{
	public partial class Bullet : Node3D
	{
		[Export]
		public float speed = 6;

		public Vector3 velocity;

		public override void _Process(double delta)
		{
			base._Process(delta);

			Position += velocity.Normalized() * speed * (float)delta;
		}
	}
}
