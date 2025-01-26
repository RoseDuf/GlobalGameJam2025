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
    public partial class DebrisSpawnManager : Node
    {
        [Export] public ObstacleData obstacleData;

        [Export] private Timer _obstacleSpawnTimer;

        private int _currentWaveIndex;

        private List<Obstacle> _currentObstacles = new();

        private float _timeToStopWave = 0;
        private float _timer = 0;
        private bool _canSpawn = false;

        public void StartDebrisWave(float timeToStopWave)
        {
            _timeToStopWave = timeToStopWave;
            _canSpawn = true;
        }

        public override void _Process(double delta)
        {
            if (_canSpawn)
            {
                _timer += (float)delta;

                if (_timer < _timeToStopWave)
                {
                    _timer = 0;
                    _canSpawn = false;
                }
            }
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

            Obstacle obstacle = obstacleData.obstacleScene.Instantiate<Obstacle>();
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
