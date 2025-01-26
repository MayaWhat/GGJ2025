using Godot;

public partial class EndTrigger : Area2D
{
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.TheEnd();
		}
	}
}
