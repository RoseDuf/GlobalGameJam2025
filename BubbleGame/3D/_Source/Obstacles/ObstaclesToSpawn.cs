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
        [Export] public int obstacleType; // kinda an enum, for which resourse to spawn from list
    }
}
