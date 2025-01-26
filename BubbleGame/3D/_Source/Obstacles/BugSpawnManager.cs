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
        [Export] private MeshInstance3D planeMesh;
        [Export] private BugData _bugData;
        [Export] public BugSwarmData[] swarms;
        [Export] private Timer _obstacleSpawnTimer;

        public delegate void OnBugSwarmStart();
        public event OnBugSwarmStart BugSwarmStartHandler;

        public delegate void OnBugSwarmEnd(float TimeToStopSwarm);
        public event OnBugSwarmEnd BugSwarmEndHandler;

        private int _currentSwarmIndex;

        private List<Obstacle> _currentObstacles = new();

        private RandomNumberGenerator _rng = new RandomNumberGenerator();

        public override void _Ready()
        {
            if (swarms != null && swarms.Length > 0)
            {
                StartTimerForNextSwarm(0);
            }
        }

        public override void _Process(double delta)
        {
        }

        private void StartTimerForNextSwarm(int swarmIndex)
        {
            if (_currentSwarmIndex >= swarms.Length)
            {
                // End of 3D happens here technically
                return;
            }

            _obstacleSpawnTimer.WaitTime = 3; //swarms[_currentSwarmIndex].timeUntilSwarmStarts;
            _obstacleSpawnTimer.Start();
        }

        private void OnSpawnObstacles()
        {
            _obstacleSpawnTimer.Stop();

            BugSwarmData currentSwarm = swarms[_currentSwarmIndex];
            for (int i = 0; i < currentSwarm.obstacles.Length; i++)
            {
                for (int j = 0; j < currentSwarm.obstacles[i].numberToSpawn; j++)
                {
                    SpawnObstacle(currentSwarm.obstacles[i]);
                }
            }
        }

        private void SpawnObstacle(ObstaclesToSpawn obstacleToSpawn)
        {
            if (_bugData == null || _bugData.bugTypes == null || _bugData.bugTypes.Length == 0 || obstacleToSpawn.obstacleType >= _bugData.bugTypes.Length)
            {
                return;
            }

            // Get the radius of the circular spawn area (based on the X and Z scale of the plane)
            Vector3 planeScale = planeMesh.Scale;
            float radius = planeScale.X;  // Assuming X and Z have the same scale for a circular area

            // Generate random polar coordinates
            float angle = _rng.RandfRange(0f, 2 * Mathf.Pi);  // Random angle in radians
            float randomRadius = _rng.RandfRange(0f, radius); // Random radius within the circle

            // Convert polar coordinates to Cartesian coordinates (x, z)
            float x = Mathf.Cos(angle) * randomRadius;
            float y = Mathf.Sin(angle) * randomRadius;

            // Generate random depth (z)
            float z = _rng.RandfRange(0, -swarms[_currentSwarmIndex].swarmDepth);

            // Generate the final position
            Vector3 randomPosition = new Vector3(x, y, z);

            // Transform random position to world space
            Transform3D planeTransform = planeMesh.GlobalTransform;
            randomPosition = planeTransform * randomPosition;

            // TODO make this bug class
            ObstacleData obstacleData = _bugData.bugTypes[obstacleToSpawn.obstacleType];

            Obstacle obstacle = obstacleData.obstacleScene.Instantiate<Obstacle>();
            this.AddChild(obstacle);
            obstacle.GlobalTransform = new Transform3D(obstacle.GlobalTransform.Basis, randomPosition);

            obstacle.Initialize(obstacleData);

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

            if (_currentObstacles.Count == 0 && swarms != null && swarms.Length > _currentSwarmIndex)
            {
                _currentSwarmIndex++;
                StartTimerForNextSwarm(_currentSwarmIndex);
                BugSwarmEndHandler.Invoke(3);
            }
        }
    }
}
