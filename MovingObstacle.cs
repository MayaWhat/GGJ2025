using Godot;
using System;

public partial class MovingObstacle : Node2D
{
	[Export] public bool PopsPlayer { get; private set; }

	public override void _Ready()
	{
		GetNodeOrNull<PlayerPopper>("%PlayerPopper")?.TogglePopping(PopsPlayer);
	}
}
