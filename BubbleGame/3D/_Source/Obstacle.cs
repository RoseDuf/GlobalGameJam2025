using Godot;
using System;
using System.Collections.Generic;

namespace BubbleGame._3D
{
	public partial class Obstacle : Node3D
	{
		struct ObstacleData
		{
			public float speed;
			public float time;
		}

		private List<ObstacleData> _obstacles = new()
		{
			new ObstacleData { speed = 1, time = 0 },
			new ObstacleData { speed = 2, time = 5 },
			new ObstacleData { speed = 3, time = 10 },
		};

		private List<(int, int)[]> _waves = new()
		{
			new (int, int)[] { (0, 5), (1, 2), (2, 3) },
			new (int, int)[] { (0, 1), (1, 0), (2, 0) },
		};

		[Export] private PackedScene _enemyScene;

		private List<int> _waveData = new();
		private int _currentWavePosition;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{


		}

		private void StartWave(int waveIndex)
		{
			_waveData.Clear();
			foreach ((int type, int count) in _waves[waveIndex])
			{
				for (int i = 0; i < count; i++)
				{
					_waveData.Add(type);

				}
			}

			_currentWavePosition = 0;
		}
	}
}
