using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleGame._3D
{
    [GlobalClass]
    public partial class DebrisData : Resource
    {
        [Export] public ObstacleData[] debrisTypes;
    }
}
