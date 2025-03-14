using System;
using Godot;

public partial class Player : RigidBody2D
{
	public static Player Instance { get; private set; }

	public const float Speed = 200.0f;
	public const float JumpVelocity = -400.0f;

	public const float BubbleLightEnergy = 0.5f;
	public const float PoppedLightEnergy = 0.25f;

	[Export] public PlayerStats Stats { get; private set; }

	[Export] public bool InBubble = false;
	private bool _hasEnteredABubble;

	public bool IsGrounded = false;

	[Export] private float Floatation = -5.0f;

	[Export] private float Gravity = 400f;

	[Export] private float MinBubbleVelocity = -50f;

	[Export] private float TargetBubbleVelocityY = -150f;

	[Export] private float MaxBubbleVelocityY = -450f;

	[Export] private float MaxBubbleVelocityX = 300f;

	[Export]
	private Node2D _moon;

	private Vector2 _originalPosition;

	private Sprite2D _bubbleSprite;

	private Vector2 _previousPosition;

	private Sprite2D _shrimpSprite;

	private CollisionShape2D _bubbleCollider;

	private AnimationPlayer _animationPlayer;

	private Light2D _light;

	private Camera2D _camera;

	private bool _justChangedBubbleState = false;

	private bool _isPushing = false;

	private bool _isShrimpRotated = false;

	private bool _isShrimpFacingLeft = false;

	private bool _isStunned = false;

	private double _stuntimer = 0;

	private Vector2 _windDirection;

	private float _windSpeed;

	private bool _isWindy = false;

	private bool _ending = false;

	private bool _ended = false;

	private ulong _startingMs;

	[Export] private AudioStream PopSound;
	[Export] private AudioStream WinSound;
	[Export] private PackedScene _bloodParticlesScene;

	[Signal]
	public delegate void BubbledEventHandler();

	[Signal]
	public delegate void NewLevelEventHandler(int level);

	[Signal]
	public delegate void PoppedEventHandler();
	[Signal]
	public delegate void StunnedEventHandler();

	[Signal]
	public delegate void MoonedEventHandler();

	public override void _Ready()
	{
		Instance = this;

		_originalPosition = GlobalPosition;
		_bubbleSprite = GetNode<Sprite2D>("%BubbleSprite");
		_shrimpSprite = GetNode<Sprite2D>("%ShrimpSprite");
		_bubbleCollider = GetNode<CollisionShape2D>("%BubbleCollider");
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		_light = GetNode<Light2D>("%Light");
		_camera = GetNode<Camera2D>("%Camera2D");
		_startingMs = Time.GetTicksMsec();

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
			return;
		}

		if (!InBubble && _hasEnteredABubble)
		{
			var blood = _bloodParticlesScene.Instantiate<GpuParticles2D>();
			blood.Emitting = true;
			AddChild(blood);
			StunMe();
		}
	}
	private void StunMe()
	{
		if (!_isStunned)
		{
			EmitSignal(SignalName.Stunned);
			_animationPlayer.Play("Falling");
			_isStunned = true;
			_stuntimer = 1.25f;
		}
	}

	public void PopMe()
	{
		if (!InBubble) return;

		_light.Energy = PoppedLightEnergy;
		InBubble = false;
		_bubbleSprite.Visible = false;

		SoundPlayer.Instance.Play(PopSound, volume: 10, randomPitch: true);
		EmitSignal(SignalName.Popped);

		_justChangedBubbleState = true;
		_bubbleCollider.SetDeferred("disabled", true);

		_shrimpSprite.Rotation = 0;
		_isShrimpRotated = false;
		GetNode<GpuParticles2D>("%PopParticles").Emitting = true;

		StunMe();
	}


	public void BubbleMe()
	{
		if (!InBubble)
		{
			_light.Energy = BubbleLightEnergy;
			InBubble = true;
			_hasEnteredABubble = true;
			_bubbleSprite.Visible = true;
			IsGrounded = false;
			EmitSignal(SignalName.Bubbled);
			_bubbleCollider.SetDeferred("disabled", false);

			_justChangedBubbleState = true;
		}
	}

	public void LevelTransition(int level)
	{
		EmitSignal(SignalName.NewLevel, level);
	}

	public void RestrictCamera()
	{
		_camera.LimitLeft = -420;
		_camera.LimitRight = 1500;
	}

	public void FreeCamera()
	{
		_camera.LimitLeft = -10000000;
		_camera.LimitRight = 10000000;

	}

	public void WindMe(Vector2 direction, float Windiness = 100f)
	{
		_isWindy = true;
		_windDirection = direction;
		_windSpeed = Windiness;
	}

	public void UnWindMe()
	{
		_isWindy = false;
	}

	private Vector2 LastPosition = new Vector2();

	public override void _Process(double delta)
	{
		if (_ended)
		{
			return;
		}

		Stats.UpdateTimer(TimeSpan.FromMilliseconds(Time.GetTicksMsec() - _startingMs));

		if (_previousPosition != GlobalPosition)
		{
			var score = GlobalPosition.Y * -1;
			Stats.UpdateCurrentScore((int)score);
			if (score > Stats.HighScore) { Stats.UpdateHighScore((int)score); }
			_previousPosition = GlobalPosition;
		}

		if (_isStunned)
		{
			_stuntimer -= delta;
			if (_stuntimer < 0)
			{
				_isStunned = false;
			}
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
		var direction = new Vector2();

		if (_ended)
		{
			return;
		}

		if (_ending)
		{
			GlobalPosition = GlobalPosition.MoveToward(_moon.GlobalPosition, (float)delta * 500f);
			if (GlobalPosition.IsEqualApprox(_moon.GlobalPosition))
			{
				ActuallyTheEnd();
			}
			return;
		}

		if (!_isStunned)
		{
			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			direction = Input.GetVector("left", "right", "up", "down");
		}

		if (direction.X > 0)
		{
			_shrimpSprite.Scale = new Vector2(Math.Abs(_shrimpSprite.Scale.X), _shrimpSprite.Scale.Y);
			_isShrimpFacingLeft = false;
		}
		else if (direction.X < 0)
		{
			_shrimpSprite.Scale = new Vector2(-Math.Abs(_shrimpSprite.Scale.X), _shrimpSprite.Scale.Y);
			_isShrimpFacingLeft = true;
		}

		if (InBubble)
		{
			BubblyMovement(delta, direction);

			if (direction.Y > 0)
			{
				if (_isShrimpFacingLeft)
				{
					_shrimpSprite.Rotation = 0;
					_shrimpSprite.Rotate(1.5708f * 3);
				}
				else
				{
					_shrimpSprite.Rotation = 0;
					_shrimpSprite.Rotate(1.5708f);
				}
			}
			else if (direction.Y <= 0)
			{
				_shrimpSprite.Rotation = 0;
				_isShrimpRotated = false;
			}

		}
		else
		{
			StillMovement(delta, direction);
		}

		if (_isWindy)
		{
			ApplyImpulse(_windDirection * _windSpeed);
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
			ApplyImpulse(new Vector2(0, direction.Y) * 4);
		}
		else if (direction.Y < 0 && LinearVelocity.Y > MaxBubbleVelocityY)
		{
			// Up
			ApplyImpulse(new Vector2(0, direction.Y) * 10);
		}

		// Left right
		if ((direction.X < 0 && LinearVelocity.X > -MaxBubbleVelocityX) || (direction.X > 0 && LinearVelocity.X < MaxBubbleVelocityX))
		{
			ApplyImpulse(new Vector2(direction.X, 0) * 3);
		}

		var shrimpFactorX = Mathf.MoveToward(_shrimpSprite.Position.X, direction.X * 10, 1);
		var shrimpFactorY = Mathf.MoveToward(_shrimpSprite.Position.Y, direction.Y * 10, 1);

		var shrimpVector = new Vector2(shrimpFactorX, shrimpFactorY);
		_shrimpSprite.Position = shrimpVector;

		_bubbleSprite.Scale = new Vector2(1 + Math.Abs(shrimpFactorX) / 100, 1 + Math.Abs(shrimpFactorY) / 100);

		if (InBubble && direction.Length() > 0)
		{
			if (!_isPushing)
			{
				_animationPlayer.Play("Push");
				_isPushing = true;
			}
		}
		else
		{
			_isPushing = false;
			_animationPlayer.Play("RESET");
		}

		//GD.Print("X: " + LinearVelocity.X + ", Y: " + LinearVelocity.Y + " DirectionY: " + direction.Y);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Reset"))
		{
			PopMe();
			GlobalPosition = _originalPosition;
		}

		if (@event.IsActionPressed("BubbleMe"))
		{
			BubbleMe();
		}

		if (@event.IsActionPressed("PopMe"))
		{
			PopMe();
		}

		if (@event.IsActionPressed("Exit"))
		{
			GetTree().Quit();
		}
	}

	public void TheEnd()
	{
		_ending = true;
		SetDeferred(PropertyName.Freeze, true);
		_shrimpSprite.Rotation = 0;
		_animationPlayer.Play("RESET");
		_isPushing = false;
	}

	public void ActuallyTheEnd()
	{
		SoundPlayer.Instance.Play(WinSound, -5);
		EmitSignal(SignalName.Mooned);
		Stats.End();
		_animationPlayer.Play("Finish");
		_ended = true;
	}
}
