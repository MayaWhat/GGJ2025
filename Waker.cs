using Godot;

public partial class Waker : Area2D
{
	[Signal] public delegate void WakeUpEventHandler();

	private Vector2 originalPosition;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		originalPosition = GlobalPosition;
	}

	private void OnAreaEntered(Area2D area)
	{

		if (area.GetParent() is Player)
		{
			EmitSignal(SignalName.WakeUp);
		}
	}

	public override void _Process(double delta)
	{
		GlobalPosition = originalPosition;
	}
}
