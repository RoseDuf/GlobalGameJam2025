using Godot;
using System;
using System.Collections.Generic;

namespace BubbleGame._3D
{
	public partial class Obstacle : Node3D
	{
		public struct ObstacleData
		{
			public float speed;
			public float time;
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{


		}

		public void Initialize(ObstacleData obstacleData)
		{

		}

    }
}
