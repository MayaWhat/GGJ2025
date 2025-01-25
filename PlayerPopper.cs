using Godot;

public partial class PlayerPopper : Area2D
{
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	public void TogglePopping(bool toggle)
	{
		Monitoring = toggle;
		Monitorable = toggle;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.PopMe();

			if (GetParent() is ComplexShark shark)
			{
				shark.Chill();
			}
		}
	}
}
