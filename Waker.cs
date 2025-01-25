using Godot;

public partial class Waker : Area2D
{
	[Signal] public delegate void WakeUpEventHandler();

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player)
		{
			EmitSignal(SignalName.WakeUp);
		}
	}
}
