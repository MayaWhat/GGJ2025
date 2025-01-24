using Godot;
using System;

public partial class UI : CanvasLayer
{
	private Label _highScoreLabel;
	private Label _currentScoreLabel;

	[Export] private PlayerStats _playerStats;


	public override void _Ready()
	{
		_highScoreLabel = GetNode<Label>("%HighScore");
		_currentScoreLabel = GetNode<Label>("%CurrentScore");
		_playerStats.ScoreChanged += OnPlayerScoreChanged;
	}

	private void OnPlayerScoreChanged()
	{
		_highScoreLabel.Text = _playerStats.HighScore.ToString();
		_currentScoreLabel.Text = _playerStats.CurrentScore.ToString();
	}
}
