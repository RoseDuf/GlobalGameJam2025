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
        [Export] private WaveData[] _waves;

        [Export] private PackedScene _obstacleScene;
        [Export] private Timer _obstacleSpawnTimer;

        private int _currentWaveIndex;

        private List<Obstacle> _currentObstacles = new();

        public override void _Ready()
        {
            if (_waves != null && _waves.Length > 0)
            {
                StartNewWave(0);
            }
        }

        public override void _Process(double delta)
        {
        }

        private void StartNewWave(int waveIndex)
        {
            if (_waves == null || _currentWaveIndex >= _waves.Length)
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
            foreach (ObstaclesToSpawn obstacleToSpawn in currentWave.obstacles)
            {
                for (int i = 0; i < obstacleToSpawn.numberToSpawn; i++)
                {
                    SpawnObstacleFromList(obstacleToSpawn);
                }
            }
        }

        private void SpawnObstacleFromList(ObstaclesToSpawn obstacleToSpawn)
        {
            ObstacleData obstacleData = obstacleToSpawn.obstacleData;

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
            }

            if (_currentObstacles.Count == 0 && _waves != null && _waves.Length > _currentWaveIndex)
            {
                _currentWaveIndex++;
                StartNewWave(_currentWaveIndex);
            }
        }
    }
}
