using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public bool InBubble = false;
	[Export] private float Gravity = -100.0f;

	[Export] private float MinBubbleVelocity = -100f;

	[Export] private float MaxBubbleVelocity = -500f;
	private Vector2 _originalPosition;

	public override void _Ready()
	{
		_originalPosition = GlobalPosition;
	}

	public void PopMe()
	{
		Velocity = Vector2.Zero;
		GlobalPosition = _originalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Position.Y < -1800f)
		{
			Position = new Vector2();
			Velocity = new Vector2();
		}

		var velocity = Velocity;
		velocity.Y += (float)delta * Gravity;
		Velocity = velocity;

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
			// If slowing down and not already min speed
			if (direction.Y > 0 && velocity.Y < MinBubbleVelocity)
			{
				velocity.Y += direction.Y * Speed * (float)delta;
			}

			// If speeding up 
			if (direction.Y < 0)
			{
				velocity.Y += direction.Y * Speed * (float)delta;
			}
		}

		// Max speed
		if (Velocity.Y < MaxBubbleVelocity)
		{
			velocity.Y = MaxBubbleVelocity;
		}

		Velocity = velocity;
		MoveAndSlide();

		GD.Print("Pos X:" + Position.X + ", Pos Y:" + Position.Y + ". Vel X:" + Velocity.X + ", Vel Y:" + Velocity.Y);
	}
}
