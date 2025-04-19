using BubbleGame.Common.SceneManagement;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BubbleGame._3D.Obstacle;

namespace BubbleGame._3D
{
    /*
     * This class handles the bubble path the player has to navigate through. And how/when enemies/debris will appear
    */
    public partial class SpawnManager : Node
    {
        [Export] public BugSpawnManager bugSpawnManager;
        [Export] public DebrisSpawnManager debrisSpawnManager;
        [Export] public float _delayBetweenSwarms = 1;

        private Godot.Collections.Array _timeStamps;

        public delegate void OnNoMoreBugsLeftEvent();
        public event OnNoMoreBugsLeftEvent NoMoreBugsLeftHandler;

        public override void _Ready()
        {
            _timeStamps = SceneManager.Instance.GetTimeStampsCachedData();

            bugSpawnManager.BugSwarmStartHandler += OnBugSwarmStart;
            bugSpawnManager.BugSwarmEndHandler += OnBugSwarmEnd;
            bugSpawnManager.NoMoreBugsLeftHandler += OnNoMoreBugsLeft;

            if (bugSpawnManager.swarms != null || bugSpawnManager.swarms.Length > 0)
            {
                Godot.Collections.Array array = (Godot.Collections.Array)_timeStamps[0];
                debrisSpawnManager.StartDebris((float)array[0] - _delayBetweenSwarms);
            }
        }
        public override void _ExitTree()
        {
            bugSpawnManager.BugSwarmStartHandler -= OnBugSwarmStart;
            bugSpawnManager.BugSwarmEndHandler -= OnBugSwarmEnd;
            bugSpawnManager.NoMoreBugsLeftHandler -= OnNoMoreBugsLeft;
        }

        private void OnBugSwarmStart()
        {
            debrisSpawnManager.StopDebris();
        }

        private void OnBugSwarmEnd(float TimeForNextWave)
        {
            debrisSpawnManager.StartDebris(TimeForNextWave - _delayBetweenSwarms);
        }

        private void OnNoMoreBugsLeft()
        {
            NoMoreBugsLeftHandler?.Invoke();
        }
    }
}
