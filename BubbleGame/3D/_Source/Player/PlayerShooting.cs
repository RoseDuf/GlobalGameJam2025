using Godot;

namespace BubbleGame._3D
{
	/// <summary>
	/// This class controls player shooting.
	/// </summary>
	public partial class PlayerShooting : Node
	{
		[Export]
		private Node3D _playerCursor;

		[Export]
		private Node3D _player;

		[Export]
		private PackedScene _bulletNode;

		public override void _Process(double delta)
		{
			base._Process(delta);

			Vector3 shootVector = _playerCursor.Position - _player.Position;

			if (Input.IsActionJustPressed("shoot"))
			{
				Node3D bulletInstance = _bulletNode.Instantiate<Node3D>();
				GetTree().CurrentScene.AddChild(bulletInstance);
			}
		}
	}
}