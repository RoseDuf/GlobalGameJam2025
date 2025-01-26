using Godot;
using System.ComponentModel.DataAnnotations;

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
		public Cursor playerCursor;

		[Export]
		public float moveSpeed = 3;

		[Export]
		public float lerpSpeed = 3;

		[Export]
		public float topScreenClampThreshold = 1;
		[Export]
		public float bottomScreenClampThreshold = 1;
		[Export]
		public float leftScreenClampThreshold = 1;
		[Export]
		public float rightScreenClampThreshold = 1;

		private Vector3 _cursorMovement;
		private Vector3 _accumulatedCursorMovement;
		private Vector3 _lastAccumulatedCursorMovement;
		private Vector3 _lastAccumulatedCursorMovementDelta;

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);
		}

		public override void _Process(double delta)
		{
			base._Process(delta);

			_cursorMovement = Vector3.Zero;
			
			if (Input.IsAnythingPressed())
			{
				if (Input.IsActionPressed("move_left"))
				{
					_cursorMovement += new Vector3(1, 0, 0);
				}
				if (Input.IsActionPressed("move_right"))
				{
					_cursorMovement += new Vector3(-1, 0, 0);
				}
				if (Input.IsActionPressed("move_down"))
				{
					_cursorMovement += new Vector3(0, -1, 0);
				}
				if (Input.IsActionPressed("move_up"))
				{
					_cursorMovement += new Vector3(0, 1, 0);
				}
			}

			Camera3D camera = GetTree().Root.GetCamera3D();
			Vector2 viewPortMousePos = GetViewport().GetMousePosition();
			Vector3 positionOnViewPort = camera.ProjectRayOrigin(viewPortMousePos);

			// trying to get mouse movement working
			//Vector2 viewPortMousePos = GetViewport().GetMousePosition();
			//Vector3 rayOrigin = camera.ProjectRayOrigin(viewPortMousePos);
			//Vector3 rayEnd = camera.ProjectRayOrigin(viewPortMousePos);
			//Vector3 aimingRay = rayEnd - rayOrigin;
			//GD.Print($"ray: {aimingRay}");

			if ((_accumulatedCursorMovement.Y >= topScreenClampThreshold && _cursorMovement.Y > 0) ||
				(_accumulatedCursorMovement.Y <= bottomScreenClampThreshold && _cursorMovement.Y < 0))
			{
				_cursorMovement.Y = 0;
			}

			if ((_accumulatedCursorMovement.X > leftScreenClampThreshold && _cursorMovement.X > 0) ||
				(_accumulatedCursorMovement.X < rightScreenClampThreshold && _cursorMovement.X < 0))
			{
				_cursorMovement.X = 0;
			}

			Vector3 adjustment = _cursorMovement * moveSpeed * (float)delta;
			playerCursor.GlobalPosition += adjustment;

			Plane playerPlane = new Plane(Vector3.Forward, player.Position);
			Vector3 projectedPoint = playerPlane.Project(playerCursor.Position);
			Vector3 playerMovement = projectedPoint - player.Position;
			playerMovement.Z = 0;

			Vector3 desiredPosition = player.GlobalPosition + playerMovement;

			player.GlobalPosition = player.GlobalPosition.Lerp(desiredPosition, lerpSpeed * (float)delta);

			player.LookAt(playerCursor.GlobalPosition);

			_accumulatedCursorMovement += _cursorMovement;
		}
	}
}
