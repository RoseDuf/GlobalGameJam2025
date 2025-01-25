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
		public Cursor playerCursor;

		[Export]
		public float moveSpeed = 3;

		private Vector3 _cursorMovement;

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
					GD.Print("Left");
					_cursorMovement += new Vector3(moveSpeed, 0, 0);
				}
				if (Input.IsActionPressed("move_right"))
				{
					_cursorMovement += new Vector3(-moveSpeed, 0, 0);
				}
				if (Input.IsActionPressed("move_down"))
				{
					_cursorMovement += new Vector3(0, -moveSpeed, 0);
				}
				if (Input.IsActionPressed("move_up"))
				{
					_cursorMovement += new Vector3(0, moveSpeed, 0);
				}
			}

			// trying to get mouse movement working
			//Vector2 viewPortMousePos = GetViewport().GetMousePosition();
			//Camera3D camera = GetTree().Root.GetCamera3D();
			//Vector3 rayOrigin = camera.ProjectRayOrigin(viewPortMousePos);
			//Vector3 rayEnd = camera.ProjectRayOrigin(viewPortMousePos);
			//Vector3 aimingRay = rayEnd - rayOrigin;
			//GD.Print($"ray: {aimingRay}");

			playerCursor.Position += _cursorMovement * (float)delta;

			Vector3 playerMovement = Vector3.Zero;

			Plane playerPlane = new Plane(Vector3.Forward, player.Position);
			Vector3 projectedPoint = playerPlane.Project(playerCursor.Position);
			playerMovement += projectedPoint - player.Position;
			playerMovement.Z = 0;

			player.Position += playerMovement * (float)delta;

			player.LookAt(playerCursor.Position);
		}
	}
}