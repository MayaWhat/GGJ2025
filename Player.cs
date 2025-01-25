using System;
using Godot;

public partial class Player : RigidBody2D
{
	public const float Speed = 200.0f;
	public const float JumpVelocity = -400.0f;

	[Export] public PlayerStats Stats { get; private set; }

	[Export] public bool InBubble = false;

	public bool IsGrounded = false;

	[Export] private float Floatation = -5.0f;

	[Export] private float Gravity = 400f;

	[Export] private float MinBubbleVelocity = -50f;

	[Export] private float TargetBubbleVelocityY = -150f;

	[Export] private float MaxBubbleVelocityY = -450f;

	[Export] private float MaxBubbleVelocityX = 300f;

	private Vector2 _originalPosition;

	private Sprite2D _bubbleSprite;

	private Vector2 _previousPosition;

	private Sprite2D _shrimpSprite;

	private bool _justChangedBubbleState = false;

	[Export] private AudioStream PopSound;

	[Signal]
	public delegate void BubbledEventHandler();

	[Signal]
	public delegate void PoppedEventHandler();

	public override void _Ready()
	{
		_originalPosition = GlobalPosition;
		_bubbleSprite = GetNode<Sprite2D>("%BubbleSprite");
		_shrimpSprite = GetNode<Sprite2D>("%ShrimpSprite");

		BodyEntered += OnBodyEntered;
	}

	void OnBodyEntered(Node body)
	{
		// I'd rather have the SecretTiles check for detection with the player
		// but TileMaps and TileSets have no mechanism to signal collisions so
		// we have to do it in this direction
		if (body is SecretTiles secretTiles)
		{
			secretTiles.Reveal();
		}
	}

	public void PopMe()
	{
		if (!InBubble) return;

		InBubble = false;
		_bubbleSprite.Visible = false;

		SoundPlayer.Instance.Play(PopSound, volume: 10, randomPitch: true);
		EmitSignal(SignalName.Popped);

		_justChangedBubbleState = true;
	}


	public void BubbleMe()
	{
		if (!InBubble)
		{
			InBubble = true;
			_bubbleSprite.Visible = true;
			IsGrounded = false;
			EmitSignal(SignalName.Bubbled);

			_justChangedBubbleState = true;
		}
	}

	public void WindMe(Vector2 direction, float Windiness = 100f) {
		ApplyImpulse(direction * Windiness);
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

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_justChangedBubbleState)
		{
			_justChangedBubbleState = false;

			state.LinearVelocity = new Vector2();
			state.AngularVelocity = 0f;
			state.SetConstantForce(new Vector2());
		}

		if (InBubble)
		{
			GlobalRotation = 0;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Testing reset
		if (Position.Y < -1800f)
		{
			Position = _originalPosition;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (direction.X > 0)
		{
			_shrimpSprite.Scale = new Vector2(1, 1);
		}
		else if (direction.X < 0)
		{
			_shrimpSprite.Scale = new Vector2(-1, 1);
		}

		if (InBubble)
		{
			BubblyMovement(delta, direction);
		}
		else
		{
			StillMovement(delta, direction);
		}

		LastPosition = Position;
	}

	private void StillMovement(double delta, Vector2 direction)
	{
		// Left right
		if ((direction.X < 0 && LinearVelocity.X > -MaxBubbleVelocityX) || (direction.X > 0 && LinearVelocity.X < MaxBubbleVelocityX))
		{
			ApplyImpulse(new Vector2(direction.X, 0) * 5);
		}
	}

	private void BubblyMovement(double delta, Vector2 direction)
	{
		// Floatation
		if (LinearVelocity.Y > TargetBubbleVelocityY)
		{
			if (LinearVelocity.Y > MinBubbleVelocity)
			{
				ApplyImpulse(new Vector2(0, Floatation * 3));
			}
			else
			{
				ApplyImpulse(new Vector2(0, Floatation));
			}
		}

		if (direction.Y > 0)
		{
			// Down
			ApplyImpulse(new Vector2(0, direction.Y) * 5);
		}
		else if (direction.Y < 0 && LinearVelocity.Y > MaxBubbleVelocityY)
		{
			// Up
			ApplyImpulse(new Vector2(0, direction.Y) * 10);
		}

		// Left right
		if ((direction.X < 0 && LinearVelocity.X > -MaxBubbleVelocityX) || (direction.X > 0 && LinearVelocity.X < MaxBubbleVelocityX))
		{
			ApplyImpulse(new Vector2(direction.X, 0) * 5);
		}

		var shrimpFactorX = Mathf.MoveToward(_shrimpSprite.Position.X, direction.X * 10, 1);
		var shrimpFactorY = Mathf.MoveToward(_shrimpSprite.Position.Y, direction.Y * 10, 1);

		var shrimpVector = new Vector2(shrimpFactorX, shrimpFactorY);
		_shrimpSprite.Position = shrimpVector;

		_bubbleSprite.Scale = new Vector2(1 + Math.Abs(shrimpFactorX) / 100, 1 + Math.Abs(shrimpFactorY) / 100);

		//GD.Print("X: " + LinearVelocity.X + ", Y: " + LinearVelocity.Y + " DirectionY: " + direction.Y);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Reset"))
		{
			PopMe();
			GlobalPosition = _originalPosition;
		}

		if (@event.IsActionPressed("Exit"))
		{
			GetTree().Quit();
		}
	}
}
