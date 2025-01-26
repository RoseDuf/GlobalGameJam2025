using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleGame._3D
{
    [GlobalClass]
    public partial class ObstacleData : Resource
    {
        [Export] public PackedScene obstacleScene;
        [Export] public float speed;
        [Export] public float frontOfPlayerTime;
    }
}
