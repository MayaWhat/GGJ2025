using Godot;
using System;
using System.Collections.Generic;

public partial class WindyBit : Area2D
{
	[Export] private PackedScene _windLineScene;

	[Export] public float Windiness { get; set; } = 2f;

	[Export] public Vector2 Direction { get; set; } = new Vector2(1, 0);

	[Export] private CollisionShape2D _collisionShape;

	private float _width;

	private float _height;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;

		_width = (_collisionShape.Shape as RectangleShape2D).Size.X;
		_height = (_collisionShape.Shape as RectangleShape2D).Size.Y;
	}

	private void OnAreaEntered(Area2D area)
	{
		GD.Print(area);
		if (area.GetParent() is Player player)
		{
			player.WindMe(Direction, Windiness);
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.UnWindMe();
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetChildCount() < 5)
		{
			var newScene = _windLineScene.Instantiate<WindTrail>();
			AddChild(newScene);
			newScene.Direction = Direction;
			var minX = -_width / 2;
			var maxX = _width / 2;
			var randomX = GD.Randf() * (maxX - minX) + minX;

			var minY = -_height / 2;
			var maxY = _height / 2;
			var randomY = GD.Randf() * (maxY - minY) + minY;

			newScene.Position = new Vector2(randomX, randomY);
		}
	}
}
