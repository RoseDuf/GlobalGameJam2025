using BubbleGame.Common.SceneManagement;
using Godot;
using System;

public partial class Bubble3D : Node
{
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("escape"))
		{
			SceneManager.Instance.LoadScene("WelcomeScene");
		}
	}
}
