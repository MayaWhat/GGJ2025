using Godot;
using System;

public partial class MovingJellyfish : Node2D
{
	[Export] private float _moveSpeed = 5;
	
	public override void _Ready()
	{
		GetNode<ObstacleMovement>("%ObstacleMovement").MoveSpeed = _moveSpeed;
	}
}
