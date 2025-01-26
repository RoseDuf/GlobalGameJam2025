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
        [Export] public int numberToSpawn;
        [Export] public ObstacleData obstacleData;
    }
}
