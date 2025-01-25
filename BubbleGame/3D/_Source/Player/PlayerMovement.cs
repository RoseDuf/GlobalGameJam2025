using Godot;

namespace BubbleGame._3D
{
	/// <summary>
	/// This class controls player movement.
	/// </summary>
	public partial class PlayerMovement : Node
	{
		[Export]
		public Player player;

		[Export]
		public float moveSpeed;

		private Vector3 _movement;

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);

			_movement = Vector3.Zero;

			if (@event.IsAction("move_left"))
			{
				_movement += new Vector3(-moveSpeed, 0, 0);
			}
			if (@event.IsAction("move_right"))
			{
				_movement += new Vector3(moveSpeed, 0, 0);
			}
			if (@event.IsAction("move_down"))
			{
				_movement += new Vector3(0, -moveSpeed, 0);
			}
			if (@event.IsAction("move_up"))
			{
				_movement += new Vector3(0, moveSpeed, 0);
			}
		}

		public override void _Process(double delta)
		{
			base._Process(delta);

			player.Position += _movement;
		}
	}
}