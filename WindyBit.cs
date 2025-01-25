using Godot;
using System;

public partial class WindyBit : Area2D
{
	[Export] public int Windiness { get; set; }

	[Export] public Vector2 Direction { get; set; } = new Vector2(1, 0);

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
			player.WindMe(Direction, Windiness);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
