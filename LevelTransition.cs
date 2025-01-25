using Godot;

public partial class LevelTransition : Area2D
{
    [Export]
    public int Level { get; set; }

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.LevelTransition(Level);
		}
	}
}
