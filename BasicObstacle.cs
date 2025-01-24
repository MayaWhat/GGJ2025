using Godot;
using System;

public partial class BasicObstacle : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		GD.Print(area);
		if (area.GetParent() is Player player)
		{
			player.PopMe();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
