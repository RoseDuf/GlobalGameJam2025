using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleGame._3D
{
    [GlobalClass]
    public partial class BugData : Resource
    {
        [Export] public ObstacleData[] bugTypes;
    }
}
