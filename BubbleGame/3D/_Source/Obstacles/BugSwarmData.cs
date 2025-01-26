using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace BubbleGame._3D
{
    [GlobalClass]
    public partial class BugSwarmData : Resource
    {
        [Export(PropertyHint.Range, "0.0, 50.0")] public float swarmDepth;
        [Export] public ObstaclesToSpawn[] obstacles; // first int is obstacle type, second is how many of that type
    }
}
