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

        public override void _Ready()
        {
            bugSpawnManager.BugSwarmStartHandler += OnBugSwarmStart;
            bugSpawnManager.BugSwarmEndHandler += OnBugSwarmEnd;

            if (bugSpawnManager.swarms != null || bugSpawnManager.swarms.Length > 0)
            {
                debrisSpawnManager.StartDebris(/*bugSpawnManager.swarms[0].timeUntilSwarmStarts*/ 3 - _delayBetweenSwarms);
            }
        }
        public override void _ExitTree()
        {
            bugSpawnManager.BugSwarmStartHandler -= OnBugSwarmStart;
            bugSpawnManager.BugSwarmEndHandler -= OnBugSwarmEnd;
        }

        private void OnBugSwarmStart()
        {
            debrisSpawnManager.StopDebris();
        }

        private void OnBugSwarmEnd(float TimeForNextWave)
        {
            debrisSpawnManager.StartDebris(TimeForNextWave - _delayBetweenSwarms);
        }
    }
}
