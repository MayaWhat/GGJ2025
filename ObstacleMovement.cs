using Godot;
using System.Collections.Generic;

public partial class ObstacleMovement : Node
{
	[Export] private Node2D _self;

	private bool _followPath;
	private ObstaclePath _obstaclePath;
	private List<Marker2D> _actualPath = new List<Marker2D>();
	private int _currentPathIndex;

	public override void _Ready()
	{
		CallDeferred(nameof(LateInit));
	}

	private void LateInit()
	{
		foreach (var child in _self.GetChildren())
		{
			if (child is ObstaclePath path)
			{
				GD.Print("Move");
				_obstaclePath = path;
				_actualPath = _obstaclePath.Path;
				_followPath = true;
				break;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (!_followPath || _actualPath.Count == 0) { return; }

		_self.GlobalPosition = _self.GlobalPosition.MoveToward(_actualPath[_currentPathIndex].GlobalPosition, 10);

		if (_self.GlobalPosition.DistanceTo(_actualPath[_currentPathIndex].GlobalPosition) < 1)
		{
			_currentPathIndex++;
			if (_currentPathIndex >= _actualPath.Count)
			{
				if (_obstaclePath.MovementType is ObstacleMovementType.Wrap or ObstacleMovementType.Reverse) { _currentPathIndex = 0; }
				if (_obstaclePath.MovementType == ObstacleMovementType.Reverse) { _actualPath.Reverse(); }
				if (_obstaclePath.MovementType == ObstacleMovementType.End) { _followPath = false; }
			}
		}
	}
}
