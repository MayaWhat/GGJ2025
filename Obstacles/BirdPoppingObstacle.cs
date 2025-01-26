using Godot;
using System;

public partial class BirdPoppingObstacle : Node2D
{
	[Export] private float _moveSpeed = 5;
	
	public override void _Ready()
	{
		GetNode<ObstacleMovement>("%ObstacleMovement").MoveSpeed = _moveSpeed;
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		_animationPlayer.Play("Flap");
	}

	private AnimationPlayer _animationPlayer;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
