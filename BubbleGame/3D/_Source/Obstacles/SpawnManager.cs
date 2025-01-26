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
        [Export] public float _delayBetweenWaves = 1;

        public override void _Ready()
        {
            bugSpawnManager.BugWaveStartHandler += OnBugWaveStart;
            bugSpawnManager.BugWaveEndHandler += OnBugWaveEnd;
            
        }
        public override void _ExitTree()
        {
            bugSpawnManager.BugWaveStartHandler -= OnBugWaveStart;
            bugSpawnManager.BugWaveEndHandler -= OnBugWaveEnd;
        }

        private void OnBugWaveStart()
        {
        }

        private void OnBugWaveEnd(float TimeForNextWave)
        {
            debrisSpawnManager.StartDebrisWave(TimeForNextWave - _delayBetweenWaves);
        }
    }
}
