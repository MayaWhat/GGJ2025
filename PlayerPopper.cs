using Godot;

public partial class PlayerPopper : Node
{
	[Export] private Area2D _area;

	public override void _Ready()
	{
		_area.AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.PopMe();
		}
	}
}
