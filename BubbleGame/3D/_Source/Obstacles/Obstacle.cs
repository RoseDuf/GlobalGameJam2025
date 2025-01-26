using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

namespace BubbleGame._3D
{
	public partial class Obstacle : Node3D
	{
        public delegate void OnObstacleDestroyedEvent(Obstacle obstacle);
        public event OnObstacleDestroyedEvent ObstacleDestroyedEventHandler;

        [Export] private Area3D _colliderArea;

        private ObstacleData _data;
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
        }

        public void Initialize(ObstacleData obstacleData)
		{
			_data = obstacleData;
        }

        private void OnAreaEntered(Area3D area)
        {
            if (area.IsInGroup("despawner"))
            {
                this.GetParent().CallDeferred(MethodName.RemoveChild, this);
                this.QueueFree();
            }
        }
    }
}
