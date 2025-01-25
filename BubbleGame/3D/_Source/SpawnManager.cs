using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleGame._3D
{
    /*
     * This class handles the bubble path the player has to navigate through. And how/when enemies/debris will appear
    */
    public partial class SpawnManager : Node
    {
        private List<Obstacle.ObstacleData> _obstacles = new()
        {
            new Obstacle.ObstacleData { speed = 1, time = 3 },
            new Obstacle.ObstacleData { speed = 2, time = 5 },
            new Obstacle.ObstacleData { speed = 3, time = 10 },
        };

        private List<(int, int)[]> _waves = new()
        {
            new (int, int)[] { (0, 1) },
            new (int, int)[] { (0, 1), (1, 0), (2, 0) },
        };

        [Export] private PackedScene _obstacleScene;
        [Export] private Timer _obstacleSpawnTimer;

        private List<int> _waveData = new();
        private int _currentWavePosition;
        private int _currentWaveIndex;

        public override void _Ready()
        {
            StartWave(0);
        }

        public override void _Process(double delta)
        {
        }

        private void StartWave(int waveIndex)
        {
            _currentWaveIndex = waveIndex;

            _waveData.Clear();
            foreach ((int type, int count) in _waves[waveIndex])
            {
                for (int i = 0; i < count; i++)
                {
                    _waveData.Add(type);

                }
            }

            _currentWavePosition = 0;

            int obstacleType = _waveData[_currentWavePosition];
            Obstacle.ObstacleData obstacleData = _obstacles[obstacleType];
            _obstacleSpawnTimer.WaitTime = obstacleData.time;

            _obstacleSpawnTimer.Start();
        }

        private void OnSpawnObstacle()
        {
            if (_currentWavePosition >= _waveData.Count())
            {
                return;
            }

            int obstacleType = _waveData[_currentWavePosition];
            Obstacle.ObstacleData obstacleData = _obstacles[obstacleType];

            Obstacle obstacle = _obstacleScene.Instantiate<Obstacle>();
            this.AddChild(obstacle);

            obstacle.Initialize(obstacleData);

            _currentWavePosition++;
        }
    }
}
