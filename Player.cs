using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	[Export] private float Gravity = -100.0f;

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		velocity.Y += (float)delta * Gravity;
		Velocity = velocity;

		// Add the gravity.
		//if (!IsOnFloor())
		//{
			//velocity += GetGravity() * (float)delta;
		//}

		// Handle Jump.
		// if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		// {
		// 	velocity.Y = JumpVelocity;
		// }

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (direction.X != 0)
		{
			velocity.X += direction.X * Speed * (float)delta;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, 2);
		}

		if (direction.Y != 0)
		{
			velocity.Y += direction.Y * Speed * (float)delta;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
