using Godot;
using System;

public partial class ComplexShark : Node2D
{
	private Node2D _visuals;

	private bool _awake = false;
	public override void _Ready()
	{
		CallDeferred(nameof(LateInit));
		_visuals = GetNode<Node2D>("%Visuals");
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
		_visuals.Visible = true;
		_awake = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_awake)
		{
			Position = Position.MoveToward(Player.Instance.Position, 20);
			LookAt(Player.Instance.GlobalPosition);
		}
	}
}
