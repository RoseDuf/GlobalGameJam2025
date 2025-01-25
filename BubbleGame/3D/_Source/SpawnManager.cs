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
            new Obstacle.ObstacleData { speed = 3, timeUntilMoving = 1 },
            new Obstacle.ObstacleData { speed = 2, timeUntilMoving = 2 },
            new Obstacle.ObstacleData { speed = 3, timeUntilMoving = 0 },
        };

        struct WaveData
        {
            public int timeUntilWaveStarts; // Time it will take for this wave to start (starting from the last one)
            public List<(int, int)> obstacles; // first int is obstacle type, second is how many of that type

            public WaveData(int _timeUntilWaveStarts, List<(int, int)> _obstacles)
            {
                timeUntilWaveStarts = _timeUntilWaveStarts;
                obstacles = _obstacles;
            }
        } 

        private List<WaveData> _waves = new List<WaveData>
        {
            new WaveData(1, new List<(int, int)>{ (0, 1) }),
            new WaveData(2, new List<(int, int)>{(0, 1), (1, 10), (2, 0) }),
        };

        [Export] private PackedScene _obstacleScene;
        [Export] private Timer _obstacleSpawnTimer;

        private int _currentWaveIndex;

        private List<Obstacle> _currentObstacles = new();

        public override void _Ready()
        {
            if (_waves.Count > 0)
            {
                StartNewWave(0);
            }
        }

        public override void _Process(double delta)
        {
        }

        private void StartNewWave(int waveIndex)
        {
            if (_currentWaveIndex >= _waves.Count)
            {
                return;
            }

            _obstacleSpawnTimer.WaitTime = _waves[_currentWaveIndex].timeUntilWaveStarts;
            _obstacleSpawnTimer.Start();
        }

        private void OnSpawnObstacles()
        {
            _obstacleSpawnTimer.Stop();

            WaveData currentWave = _waves[_currentWaveIndex];
            foreach ((int listIndex, int count) in currentWave.obstacles)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_obstacles.Count > i)
                    {
                        SpawnObstacleFromList(listIndex);
                    }
                }
            }
        }

        private void SpawnObstacleFromList(int listIndex)
        {
            Obstacle.ObstacleData obstacleData = _obstacles[listIndex];

            Obstacle obstacle = _obstacleScene.Instantiate<Obstacle>();
            obstacle.Initialize(obstacleData);
            this.AddChild(obstacle);

            _currentObstacles.Add(obstacle);

            obstacle.ObstacleDestroyedEventHandler += OnObstacleDestroyedEvent;
        }

        private void OnObstacleDestroyedEvent(Obstacle obstacle)
        {
            if (_currentObstacles.Count > 0)
            {
                obstacle.ObstacleDestroyedEventHandler -= OnObstacleDestroyedEvent;
                _currentObstacles.Remove(obstacle);
                CallDeferred(MethodName.RemoveChild, obstacle);
                obstacle.QueueFree();
            }
            else
            {
                if (_waves.Count > _currentWaveIndex)
                {
                    StartNewWave(_currentWaveIndex);

                    _currentWaveIndex++;
                }
            }
        }
    }
}
