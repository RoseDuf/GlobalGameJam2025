using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace BubbleGame._3D
{
    [GlobalClass]
    public partial class WaveData : Resource
    {
        [Export] public int timeUntilWaveStarts; // Time it will take for this wave to start (starting from the last one)
        [Export] public ObstaclesToSpawn[] obstacles; // first int is obstacle type, second is how many of that type
    }
}
