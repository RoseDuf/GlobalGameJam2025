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
        [Export] public ObstacleData[] obstacleData;
        [Export(PropertyHint.Range, "0.0, 5.0")] private float _spawnDelayRangeMin;
        [Export(PropertyHint.Range, "0.0, 5.0")] private float _spawnDelayRangeMax;
        [Export(PropertyHint.Range, "0.0, 5.0")] private int _spawnAmountRangeMin;
        [Export(PropertyHint.Range, "0.0, 5.0")] private int _spawnAmountRangeMax;

        private int _currentWaveIndex;

        private float _timeToStopWave = 0;
        private float _timer = 0;
        private bool _canSpawn = false;

        private float _spawnDelayTime = 0;
        private float _randomTimer = 0;
        private RandomNumberGenerator _randomNumberGenerator;

        public void StartDebrisWave(float timeToStopWave)
        {
            _timeToStopWave = timeToStopWave;
            _canSpawn = true;
        }

        public void StopDebrisWave()
        {
            _timeToStopWave = 0;
            _canSpawn = false;
        }
        public override void _Ready()
        {
            _randomNumberGenerator = new RandomNumberGenerator();
        }

        public override void _Process(double delta)
        {
            if (_canSpawn)
            {
                _timer += (float)delta;

                if (_timer > _timeToStopWave)
                {
                    _timer = 0;
                    _canSpawn = false;
                }
                else
                {
                    _randomTimer += (float)delta;

                    if (_randomTimer > _spawnDelayTime)
                    {
                        ObstaclesToSpawn newObstaclesToSpawn = new ObstaclesToSpawn();
                        newObstaclesToSpawn.numberToSpawn = _randomNumberGenerator.RandiRange(_spawnAmountRangeMin, _spawnAmountRangeMax);
                        if (obstacleData != null && obstacleData.Length > 0)
                        {
                            int obstacleIndex = _randomNumberGenerator.RandiRange(0, obstacleData.Length);
                            newObstaclesToSpawn.obstacleData = obstacleData[obstacleIndex];
                        }
                        SpawnObstacle(newObstaclesToSpawn);

                        _spawnDelayTime = _randomNumberGenerator.RandfRange(_spawnDelayRangeMin, _spawnDelayRangeMax);
                        _randomTimer = 0;
                    }
                }
            }
        }

        private void SpawnObstacle(ObstaclesToSpawn obstacleToSpawn)
        {
            ObstacleData obstacleData = obstacleToSpawn.obstacleData;

            Obstacle obstacle = obstacleData.obstacleScene.Instantiate<Obstacle>();
            obstacle.Initialize(obstacleData);
            this.AddChild(obstacle);
        }
    }
}
