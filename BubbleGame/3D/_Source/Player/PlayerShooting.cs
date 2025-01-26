using Godot;

namespace BubbleGame._3D
{
	/// <summary>
	/// This class controls player shooting.
	/// </summary>
	public partial class PlayerShooting : Node3D
	{
		[Export]
		public float speed = 8;

		[Export]
		private Node3D _playerCursor;

		[Export]
		private PackedScene _bulletNode;

		[Export]
		public float heldBulletGrowFactor = 1;

		[Export]
		public Vector3 maxBulletSize = new Vector3(3, 3, 3);

		private float heldTime;

		public override void _Process(double delta)
		{
			base._Process(delta);

			Vector3 shootVector = _playerCursor.GlobalPosition - GlobalPosition;

			if (Input.IsActionPressed("shoot"))
			{
				heldTime += (float)delta;
			}
			else if (Input.IsActionJustReleased("shoot"))
			{
				Node3D bulletInstance = _bulletNode.Instantiate<Node3D>();
				GetTree().CurrentScene.AddChild(bulletInstance);
				Bullet bullet = bulletInstance as Bullet;
				bullet.GlobalPosition = GlobalPosition;

				bullet.velocity = shootVector.Normalized() * speed;

				if (heldTime > 0)
				{
					bullet.Scale = bullet.Scale * (1 + heldTime) * heldBulletGrowFactor;
					bullet.Scale.Clamp(new Vector3(0.001f, 0.001f, 0.001f), maxBulletSize);
				}

				heldTime = 0;
			}
		}
	}
}