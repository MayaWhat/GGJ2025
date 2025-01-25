using Godot;
using System;

public partial class UI : CanvasLayer
{
	private Label _highScoreLabel;
	private Label _currentScoreLabel;
	private Label _timerLabel;

	[Export] private PlayerStats _playerStats;

	private ulong _startingMs;
	private TimeSpan _currentTimer;


	public override void _Ready()
	{
		_highScoreLabel = GetNode<Label>("%HighScore");
		_currentScoreLabel = GetNode<Label>("%CurrentScore");
		_timerLabel = GetNode<Label>("%Timer");
		_playerStats.ScoreChanged += OnPlayerScoreChanged;
		_startingMs = Time.GetTicksMsec();
	}

    public override void _Process(double delta)
    {
        _currentTimer = TimeSpan.FromMilliseconds(Time.GetTicksMsec() - _startingMs);
		_timerLabel.Text = $"{(int)_currentTimer.TotalMinutes:D2}:{_currentTimer.Seconds:D2}:{_currentTimer.Milliseconds:D3}";
    }

    private void OnPlayerScoreChanged()
	{
		_highScoreLabel.Text = _playerStats.HighScore.ToString();
		_currentScoreLabel.Text = _playerStats.CurrentScore.ToString();
	}
}
