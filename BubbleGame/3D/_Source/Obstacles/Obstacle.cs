using Godot;
using System;
using System.Collections.Generic;

namespace BubbleGame._3D
{
	public partial class Obstacle : Node3D
	{
		public delegate void OnObstacleDestroyedEvent(Obstacle obstacle);
		public event OnObstacleDestroyedEvent ObstacleDestroyedEventHandler;

		[Export] private Area3D _colliderArea;

		[Export]
		public float rewardScore = 1;

		private ObstacleData _data;

		public ObstacleData data => _data;

		private float _timeSinceSpawn = 0;
		private Vector3 _moveDirection = Vector3.Zero;

		public override void _Ready()
		{
			LookAt(GlobalPosition + -Vector3.Back, Vector3.Up);
		}

		public override void _Process(double delta)
		{
			_moveDirection = Vector3.Forward;
			Translate(_moveDirection * _data.speed * (float)delta);
		}

		public override void _ExitTree()
		{
			ObstacleDestroyedEventHandler?.Invoke(this);
			GameManager.Instance.score += rewardScore;
		}

		public void Initialize(ObstacleData obstacleData)
		{
			_data = obstacleData;
		}

		private void OnAreaEntered(Area3D area)
		{
			if (area.IsInGroup("despawner"))
			{
				GetTree().CurrentScene.CallDeferred(MethodName.RemoveChild, this);
				this.QueueFree();
			}
		}
	}
}
