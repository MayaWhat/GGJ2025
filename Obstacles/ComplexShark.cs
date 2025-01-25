using Godot;
using System;

public partial class ComplexShark : Node2D
{
	private Node2D _visuals;

	private Node2D _playerPopper;

	private bool _awake = false;

	private bool _retreating = false;

	private Vector2 _startingPosition;
	public override void _Ready()
	{
		CallDeferred(nameof(LateInit));
		_visuals = GetNode<Node2D>("%Visuals");
		_playerPopper = GetNode<Node2D>("%PlayerPopper");
		_startingPosition = Position;
	}

	private void LateInit()
	{
		foreach (var child in GetChildren())
		{
			if (child is Waker waker)
			{
				_visuals.Visible = false;
				waker.WakeUp += OnWakeUp;
			}
		}
	}

	private void OnWakeUp()
	{
		GD.Print("Wake");

		_visuals.Visible = true;
		_awake = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_awake) return;

		if (!_retreating && Player.Instance.InBubble)
		{
			if ((Player.Instance.GlobalPosition - GlobalPosition).Length() > 500f)
			{
				Position = Position.MoveToward(Player.Instance.Position, 20);
			}
			else if ((Player.Instance.GlobalPosition - GlobalPosition).Length() > 400f) {
				Position = Position.MoveToward(Player.Instance.Position, 10);
			}
			else
			{
				Position = Position.MoveToward(Player.Instance.Position, 5);
			}
			_visuals.LookAt(Player.Instance.GlobalPosition);
			_playerPopper.LookAt(Player.Instance.GlobalPosition);

			if ((GlobalPosition - _startingPosition).Length() > 5000f) {
				Chill();
			}
		}
		else
		{
			if ((_startingPosition - Position).Length() < 1)
			{
				_visuals.Visible = false;
				_retreating = false;
				_awake = false;
			}
			else
			{
				Position = Position.MoveToward(_startingPosition, 20);
				_visuals.LookAt(_startingPosition);
				_playerPopper.LookAt(_startingPosition);
			}
		}
	}

	public void Chill()
	{
		_retreating = true;
	}
}
