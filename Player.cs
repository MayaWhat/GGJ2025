using Godot;

public partial class Player : CharacterBody2D
{
	public const float Speed = 200.0f;
	public const float JumpVelocity = -400.0f;

	[Export] public PlayerStats Stats { get; private set; }

	[Export] public bool InBubble = false;

	public bool IsGrounded = false;

	[Export] private float Floatation = -100.0f;

	[Export] private float Gravity = 400f;

	[Export] private float MinBubbleVelocity = -100f;

	[Export] private float MaxBubbleVelocity = -500f;
	private Vector2 _originalPosition;

	private Sprite2D _bubbleSprite;

	private Vector2 _previousPosition;


	public override void _Ready()
	{
		_originalPosition = GlobalPosition;
		_bubbleSprite = GetNode<Sprite2D>("%BubbleSprite");
	}

	public void PopMe()
	{
		InBubble = false;
		_bubbleSprite.Visible = false;
		Velocity = new Vector2();
	}


	public void BubbleMe()
	{
		if (!InBubble)
		{
			Velocity = new Vector2();
			InBubble = true;
			_bubbleSprite.Visible = true;
			IsGrounded = false;
		}
	}

	private Vector2 LastPosition = new Vector2();

	public override void _Process(double delta)
	{
		if (_previousPosition != GlobalPosition)
		{
			var score = GlobalPosition.Y * -1;
			Stats.UpdateCurrentScore((int)score);
			if (score > Stats.HighScore) { Stats.UpdateHighScore((int)score); }
			_previousPosition = GlobalPosition;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Testing reset
		if (Position.Y < -1800f)
		{
			Position = _originalPosition;
			Velocity = new Vector2();
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (InBubble)
		{
			BubblyMovement(delta, direction);
		}
		else
		{
			StillMovement(delta, direction);
		}

		LastPosition = Position;

		MoveAndSlide();

		GD.Print("Pos X:" + Position.X + ", Pos Y:" + Position.Y + ". Vel X:" + Velocity.X + ", Vel Y:" + Velocity.Y);		
	}

	private void StillMovement(double delta, Vector2 direction)
	{
		var velocity = Velocity;
		velocity.Y += (float)delta * Gravity;

		if (Position.Y == LastPosition.Y)
		{
			IsGrounded = true;
		}

		if (IsGrounded)
		{
			if (direction.X != 0)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = 0;
			}
		}
		else
		{
			if (direction.X != 0)
			{
				velocity.X += direction.X * Speed * (float)delta;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, 2);
			}
		}

		Velocity = velocity;
	}

	private void BubblyMovement(double delta, Vector2 direction)
	{
		var velocity = Velocity;
		velocity.Y += (float)delta * Floatation;

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
	}



	public override void _Input(InputEvent @event)
	{
			if (@event.IsActionPressed("Reset"))
			{
				PopMe();
				GlobalPosition = _originalPosition;
			}
	}
}
