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
    public partial class BugSpawnManager : Node
    {
        [Export] public WaveData[] waves;
        [Export] private Timer _obstacleSpawnTimer;

        public delegate void OnBugWaveStart();
        public event OnBugWaveStart BugWaveStartHandler;

        public delegate void OnBugWaveEnd(float TimeToStopWave);
        public event OnBugWaveEnd BugWaveEndHandler;

        private int _currentWaveIndex;

        private List<Obstacle> _currentObstacles = new();

        public override void _Ready()
        {
            if (waves != null && waves.Length > 0)
            {
                StartTimerForNextWave(0);
            }
        }

        public override void _Process(double delta)
        {
        }

        private void StartTimerForNextWave(int waveIndex)
        {
            if (waves == null || _currentWaveIndex >= waves.Length)
            {
                return;
            }

            _obstacleSpawnTimer.WaitTime = waves[_currentWaveIndex].timeUntilWaveStarts;
            _obstacleSpawnTimer.Start();
        }

        private void OnSpawnObstacles()
        {
            _obstacleSpawnTimer.Stop();

            WaveData currentWave = waves[_currentWaveIndex];
            foreach (ObstaclesToSpawn obstacleToSpawn in currentWave.obstacles)
            {
                for (int i = 0; i < obstacleToSpawn.numberToSpawn; i++)
                {
                    SpawnObstacle(obstacleToSpawn);
                }
            }
        }

        private void SpawnObstacle(ObstaclesToSpawn obstacleToSpawn)
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

            if (_currentObstacles.Count == 0 && waves != null && waves.Length > _currentWaveIndex)
            {
                _currentWaveIndex++;
                StartTimerForNextWave(_currentWaveIndex);
            }
        }
    }
}
