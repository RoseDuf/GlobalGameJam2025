using BubbleGame._3D;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleGame._3D
{
    [GlobalClass]
    public partial class ObstaclesToSpawn : Resource
    {
        [Export] public ObstacleData obstacleData;
        [Export] public int numberToSpawn;
    }
}
