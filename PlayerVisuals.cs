using Godot;
using System;

public partial class PlayerVisuals : Node2D
{
	private Sprite2D _shrimpSprite;
	[Export] private ShaderMaterial _hitFlashMat;

	private Timer _hitFlashTimer;
	private Timer _hitFlashTimerInterval;
	private bool _hitFlashToggle;

	public override void _Ready()
	{
		(GetParent() as Player).Stunned += OnPopped;
		_shrimpSprite = GetNode<Sprite2D>("%ShrimpSprite");
		_hitFlashTimerInterval = new Timer
		{
			Autostart = true,
			WaitTime = 0.1f
		};
		_hitFlashTimerInterval.Timeout += OnHitFlashTimerInterval;
		AddChild(_hitFlashTimerInterval);
		_hitFlashTimerInterval.Paused = true;
	}

	private void OnHitFlashTimerInterval()
	{
		_hitFlashToggle = !_hitFlashToggle;
		_hitFlashMat.SetShaderParameter("active", _hitFlashToggle);
	}

	private void OnPopped()
	{
		_hitFlashTimerInterval.Paused = false;
		if (_hitFlashTimer != null)
		{
			_hitFlashTimer.QueueFree();
		}
		_hitFlashTimer = new Timer { WaitTime = 1.25f, Autostart = true };
		_hitFlashTimer.Timeout += OnHitFlashTimerTimeout;
		AddChild(_hitFlashTimer);
	}

	private void OnHitFlashTimerTimeout()
	{
		_hitFlashTimerInterval.Paused = true;
		_hitFlashToggle = false;
		_hitFlashMat.SetShaderParameter("active", _hitFlashToggle);
	}

}
