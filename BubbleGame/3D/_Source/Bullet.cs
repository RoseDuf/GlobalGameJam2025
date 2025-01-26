using Godot;

namespace BubbleGame._3D
{
	public partial class Bullet : Node3D
	{
		public Vector3 velocity;

		[Export]
		public float timeToDespawn = 5;

		private float _despawnTimer;

		private bool _isDespawned;

		public override void _Process(double delta)
		{
			base._Process(delta);

			GlobalPosition += velocity * (float)delta;

			_despawnTimer += (float)delta;

			if (_despawnTimer >= timeToDespawn)
			{
				if (!_isDespawned)
				{
					GetTree().CurrentScene.CallDeferred(MethodName.RemoveChild, this);
					this.QueueFree();
					_isDespawned = true;
				}
			}
		}

		public void Area3D_AreaEntered(Area3D area)
		{
			if (area.Name != "PlayerArea3D" && area.Name != "Despawner")
			{
				if (area.IsInGroup("Obstacle"))
				{
					Node other = area.GetParent();

					Obstacle obstacle = other.GetScript().As<Obstacle>();

					if (obstacle.data.health == 0)
					{
						GetTree().CurrentScene.CallDeferred(MethodName.RemoveChild, other);
						other.QueueFree();
					}
					else if (obstacle.data.health > 0)
					{
						obstacle.data.health--;
					}
				}

				if (!_isDespawned)
				{
					Node parent = GetParent();
					GetTree().CurrentScene.CallDeferred(MethodName.RemoveChild, parent);
					parent.QueueFree();
					_isDespawned = true;
				}
			}
		}
	}
}
