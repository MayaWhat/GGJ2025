using System.Collections.Generic;
using System.Linq;
using Godot;

public enum ObstacleMovementType
{
	// Move from final point to first point then repeat
	Wrap,
	// From start to end then back to start then repeat
	Reverse,
	// Stop at the end
	End
}
public partial class ObstaclePath : Node
{
	[Export] public ObstacleMovementType MovementType;
	public List<Marker2D> Path { get; private set; }

	public override void _Ready()
	{
		GD.Print("path");
		Path = GetChildren().Cast<Marker2D>().ToList();
	}
}
