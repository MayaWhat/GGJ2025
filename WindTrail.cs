using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WindTrail : Line2D
{
	[Export] public Vector2 Direction { get; set; }

	[Export] private int LineSegments { get; set; } = 20;

	[Export] private float TrailLength = 1f;

	[Export] private float TrailSpeed = 0.05f;

	[Export] private float RandomYOffset = 2f;

	private List<PathFollow2D> _pathFollows = new List<PathFollow2D>();

	[Export] private Gradient _colourGradient { get; set; }

	private Path2D _path2D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_path2D = GetNode<Path2D>("Path2D");

		RandomisePath();

		for (int i = 0; i < LineSegments + 1; i++)
		{
			var newPf = new PathFollow2D();
			_path2D.AddChild(newPf);
			newPf.SetHOffset((float)i / (float)LineSegments * TrailLength - TrailLength);
			newPf.Loop = false;
			_pathFollows.Add(newPf);
		}

		Gradient = new Gradient();
		Gradient.RemovePoint(0);

		for (int i = 0; i < LineSegments; i++)
		{
			Gradient.AddPoint((float)i / (float)LineSegments, new Color(1, 1, 1, 1));
		}


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (var pf in _pathFollows)
		{
			pf.HOffset += TrailSpeed;
			if (pf.HOffset >= 0 && pf.HOffset <= 1)
			{
				pf.ProgressRatio = pf.HOffset;
			}

			if (pf.HOffset > 1)
			{
				pf.ProgressRatio = 1;
			}
		}

		// for (int i = 0; i < LineSegments + 1; i++)
		// {
		// 	var color = _colourGradient.Sample(Gradient.Offsets[i]);
		// 	var colorA = color.A * _colourGradient.Sample(_pathFollows[i].ProgressRatio).A;

		// 	Gradient.SetColor(i, new Color(color.R, color.G, color.B, colorA));
		// }

		if (_pathFollows[0]?.ProgressRatio == 1)
		{
			QueueFree();
		}

		ClearPoints();

		foreach (var pf in _pathFollows)
		{
			AddPoint(pf.GlobalPosition);
		}
	}

	private void RandomisePath()
	{
		for (int i = 0; i < _path2D.Curve.PointCount; i++)
		{
			var point = _path2D.Curve.GetPointPosition(i);
			point.Y += GD.Randf() * (RandomYOffset - -RandomYOffset) + -RandomYOffset;
			_path2D.Curve.SetPointPosition(i, point);
		}
	}
}
