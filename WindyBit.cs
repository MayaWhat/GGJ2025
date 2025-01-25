using Godot;
using System;
using System.Collections.Generic;

public partial class WindyBit : Area2D
{
	[Export] private PackedScene _windLineScene;

	[Export] public float Windiness { get; set; } = 2f;

	[Export] public Vector2 Direction { get; set; } = new Vector2(1, 0);

	[Export] private CollisionShape2D _collisionShape;

	private float _minX;
	private float _maxX;

	private float _minY;
	private float _maxY;

	private double _trailCooldown = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;

		var rectangle = _collisionShape.Shape as RectangleShape2D;

		var width = rectangle.Size.X;
		var height = rectangle.Size.Y;

		var x = _collisionShape.Position.X;
		_minX = x - width / 2;
		_maxX = x + width / 2;

		var y = _collisionShape.Position.Y;
		_minY = y - height / 2;
		_maxY = y + height / 2;
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
		if (GetChildCount() < 2 || _trailCooldown < 0)
		{
			_trailCooldown = 1;
			_trailCooldown -= delta;
			
			var newScene = _windLineScene.Instantiate<WindTrail>();
			AddChild(newScene);
			newScene.Direction = Direction;

			var randomX = (GD.Randf() * (_maxX - _minX) + _minX);
			var randomY = (GD.Randf() * (_maxY - _minY) + _minY);

			newScene.Position = new Vector2(randomX, randomY);
			var rotateTarget = newScene.GlobalPosition + Direction;
			newScene.LookAt(rotateTarget); 
		}
	}
}
