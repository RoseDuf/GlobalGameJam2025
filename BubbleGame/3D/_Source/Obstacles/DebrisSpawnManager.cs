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
        [Export] private MeshInstance3D planeMesh;
        [Export] public DebrisData debrisData;
        [Export(PropertyHint.Range, "0.0, 5.0")] private float _spawnDelayRangeMin;
        [Export(PropertyHint.Range, "0.0, 5.0")] private float _spawnDelayRangeMax;
        [Export(PropertyHint.Range, "0.0, 5.0")] private int _spawnAmountRangeMin;
        [Export(PropertyHint.Range, "0.0, 5.0")] private int _spawnAmountRangeMax;

        private float _timeToStopDebris = 0;
        private float _timer = 0;
        private bool _canSpawn = false;

        private float _spawnDelayTime = 0;
        private float _randomTimer = 0;
        private RandomNumberGenerator _rng;

        public void StartDebris(float timeToStopDebris)
        {
            _timeToStopDebris = timeToStopDebris;
            _canSpawn = true;
        }

        public void StopDebris()
        {
            _timeToStopDebris = 0;
            _canSpawn = false;
        }
        public override void _Ready()
        {
            _rng = new RandomNumberGenerator();
        }

        public override void _Process(double delta)
        {
            if (_canSpawn)
            {
                _timer += (float)delta;

                if (_timer > _timeToStopDebris)
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
                        newObstaclesToSpawn.numberToSpawn = _rng.RandiRange(_spawnAmountRangeMin, _spawnAmountRangeMax);
                        if (debrisData != null && debrisData.debrisTypes.Length > 0)
                        {
                            int obstacleIndex = _rng.RandiRange(0, debrisData.debrisTypes.Length);
                            newObstaclesToSpawn.obstacleType = obstacleIndex;
                        }
                        SpawnObstacle(newObstaclesToSpawn);

                        _spawnDelayTime = _rng.RandfRange(_spawnDelayRangeMin, _spawnDelayRangeMax);
                        _randomTimer = 0;
                    }
                }
            }
        }

        private void SpawnObstacle(ObstaclesToSpawn obstacleToSpawn)
        {
            if (debrisData == null || debrisData.debrisTypes == null || debrisData.debrisTypes.Length == 0 || obstacleToSpawn.obstacleType >= debrisData.debrisTypes.Length)
            {
                return;
            }

            // Get the radius of the circular spawn area (based on the X and Z scale of the plane)
            Vector3 planeScale = planeMesh.Scale;
            float radius = planeScale.X;  // Assuming X and Z have the same scale for a circular area

            // Generate random polar coordinates
            float angle = _rng.RandfRange(0f, 2 * Mathf.Pi);  // Random angle in radians
            float randomRadius = _rng.RandfRange(0f, radius); // Random radius within the circle

            // Convert polar coordinates to Cartesian coordinates (x, z, z)
            float x = Mathf.Cos(angle) * randomRadius;
            float y = Mathf.Sin(angle) * randomRadius;
            float z = 0;

            // Generate the final position
            Vector3 randomPosition = new Vector3(x, y, z);

            // Transform random position to world space
            Transform3D planeTransform = planeMesh.GlobalTransform;
            randomPosition = planeTransform * randomPosition;

            ObstacleData obstacleData = debrisData.debrisTypes[obstacleToSpawn.obstacleType];

            Obstacle obstacle = obstacleData.obstacleScene.Instantiate<Obstacle>();
            this.AddChild(obstacle);
            obstacle.GlobalTransform = new Transform3D(obstacle.GlobalTransform.Basis, randomPosition);

            obstacle.Initialize(obstacleData);
        }
    }
}
