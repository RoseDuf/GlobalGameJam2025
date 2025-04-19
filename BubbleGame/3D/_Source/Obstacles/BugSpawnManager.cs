using BubbleGame.Common.SceneManagement;
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

        public delegate void OnBugSwarmEnd(float TimeForNextWave);
        public event OnBugSwarmEnd BugSwarmEndHandler;

        public delegate void OnNoMoreBugsLeft();
        public event OnNoMoreBugsLeft NoMoreBugsLeftHandler;

        private int _currentSwarmIndex;
        private int _currentSwarmToSpawn;
        private float _accumulatedTime = 0;

        private List<Obstacle> _currentObstacles = new();

        private RandomNumberGenerator _rng = new RandomNumberGenerator();

        private Godot.Collections.Array _timeStamps;

        public override void _Ready()
        {
            _timeStamps = SceneManager.Instance.GetTimeStampsCachedData();

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
            if (_currentSwarmIndex >= _timeStamps.Count)
            {
                NoMoreBugsLeftHandler.Invoke();
                return;
            }

            Godot.Collections.Array timeStampArray = (Godot.Collections.Array)_timeStamps[_currentSwarmIndex];

            _currentSwarmToSpawn = (int)timeStampArray[1] % swarms.Length;
            _obstacleSpawnTimer.WaitTime = (float)timeStampArray[0] - _accumulatedTime;
            _accumulatedTime = (float)timeStampArray[0];
            _obstacleSpawnTimer.Start();
        }

        private void OnSpawnObstacles()
        {
            _obstacleSpawnTimer.Stop();

            BugSwarmData currentSwarm = swarms[_currentSwarmToSpawn];
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
            float z = _rng.RandfRange(0, -swarms[_currentSwarmToSpawn].swarmDepth);

            // Generate the final position
            Vector3 randomPosition = new Vector3(x, y, z);

            // Transform random position to world space
            Transform3D planeTransform = planeMesh.GlobalTransform;
            randomPosition = planeTransform * randomPosition;

            // TODO make this bug class
            ObstacleData obstacleData = _bugData.bugTypes[obstacleToSpawn.obstacleType];

            Obstacle obstacle = obstacleData.obstacleScene.Instantiate<Obstacle>();
			GetTree().CurrentScene.AddChild(obstacle);
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

                Godot.Collections.Array timeStampArray = (Godot.Collections.Array)_timeStamps[_currentSwarmIndex];
                float timeForNextWave = (float)timeStampArray[0] - _accumulatedTime;
                BugSwarmEndHandler.Invoke(timeForNextWave);
            }
        }
    }
}
