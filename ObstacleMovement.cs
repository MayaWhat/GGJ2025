using Godot;
using System;

public partial class ObstacleMovement : Node
{
	[Export] private Area2D _self;

	private ObstaclePath _path;
	private int _currentPathIndex;

	public override void _Ready()
	{
		foreach (var child in _self.GetChildren())
		{
			if (child is ObstaclePath path)
			{
				_path = path;
				break;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_path.Path.Count == 0) { return; }
		_self.GlobalPosition = _self.GlobalPosition.MoveToward(_path.Path[_currentPathIndex].GlobalPosition, 10);

		if (_self.GlobalPosition.DistanceTo(_path.Path[_currentPathIndex].GlobalPosition) < 1)
		// if (_self.GlobalPosition == _path.Path[_currentPathIndex].GlobalPosition)
		{
			_currentPathIndex++;
			if (_currentPathIndex >= _path.Path.Count) { _currentPathIndex = 0; }
		}
	}
}
