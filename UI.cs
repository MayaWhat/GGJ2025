using Godot;
using System;

public partial class UI : CanvasLayer
{
	private Label _highScoreLabel;
	private Label _currentScoreLabel;
	private Label _timerLabel;

	private Label _endTimeLabel;

	private AnimationPlayer _animationPlayer;

	[Export] private PlayerStats _playerStats;


	public override void _Ready()
	{
		_highScoreLabel = GetNode<Label>("%HighScore");
		_currentScoreLabel = GetNode<Label>("%CurrentScore");
		_timerLabel = GetNode<Label>("%Timer");
		_endTimeLabel = GetNode<Label>("%EndTime");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerStats.ScoreChanged += OnPlayerScoreChanged;
		_playerStats.TimerChanged += OnTimerChanged;
		_playerStats.Ended += OnEnded;
	}

	private void OnPlayerScoreChanged()
	{
		_highScoreLabel.Text = _playerStats.HighScore.ToString();
		_currentScoreLabel.Text = _playerStats.CurrentScore.ToString();
	}

	private void OnTimerChanged()
	{
		_timerLabel.Text = $"{(int)_playerStats.CurrentTime.TotalMinutes:D2}:{_playerStats.CurrentTime.Seconds:D2}:{_playerStats.CurrentTime.Milliseconds:D3}";
	}

	public void OnEnded()
	{
		_endTimeLabel.Text = _timerLabel.Text;
		_animationPlayer.Play("End");
	}
}
