using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

namespace BubbleGame._3D
{
	public partial class Obstacle : Node3D
	{
		public struct ObstacleData
		{
			public float speed;
			public float timeUntilMoving;
		}

        // Step 1: Define a delegate type
        public delegate void OnObstacleDestroyedEvent(Obstacle obstacle);

        // Step 2: Declare an event using the delegate
        public event OnObstacleDestroyedEvent ObstacleDestroyedEventHandler;

        [Export] private Area3D _colliderArea;

        private ObstacleData _data;
        private float _timeSinceSpawn = 0;
        private Vector3 _moveDirection = Vector3.Zero;

        public override void _Ready()
        {
            LookAt(Vector3.Back, Vector3.Up);
        }

		public override void _Process(double delta)
		{
			// Move obstacle when time for moving is reached
			if (_timeSinceSpawn >= _data.timeUntilMoving)
			{
                _moveDirection = Vector3.Forward;
                Translate(_moveDirection * _data.speed * (float)delta);
            }

            _timeSinceSpawn += (float)delta;

        }

        public override void _ExitTree()
        {
        }

        public void Initialize(ObstacleData obstacleData)
		{
			_data = obstacleData;
        }

        private void OnAreaEntered(Area3D area)
        {
            if (area.IsInGroup("despawners"))
            {
                ObstacleDestroyedEventHandler?.Invoke(this);
            }
        }
    }
}
