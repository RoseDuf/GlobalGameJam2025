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

		public override void _Process(double delta)
		{
			base._Process(delta);

			Vector3 shootVector = _playerCursor.GlobalPosition - GlobalPosition;

			if (Input.IsActionJustPressed("shoot"))
			{
				Node3D bulletInstance = _bulletNode.Instantiate<Node3D>();
				GetTree().CurrentScene.AddChild(bulletInstance);
				Bullet bullet = bulletInstance as Bullet;
				bullet.GlobalPosition = GlobalPosition;

				bullet.velocity = shootVector.Normalized() * speed;
			}
		}
	}
}