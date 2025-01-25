using Godot;

public partial class BubbleGenerator : Node2D
{
	[Export] private PackedScene _bubbleScene;
	[Export] private float _interval = 6;

	public override void _Ready()
	{
		var timer = new Timer
		{ 
			Autostart = true,
			WaitTime = _interval
		};

		timer.Timeout += OnTimeout;
		AddChild(timer);
	}

	private void OnTimeout()
	{
		var scene = _bubbleScene.Instantiate();
		AddChild(scene);
	}
}
