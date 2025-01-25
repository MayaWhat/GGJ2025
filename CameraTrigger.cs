using Godot;

public partial class CameraTrigger : Area2D
{
    [Export]
    public bool Restrict { get; set; }

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			if (Restrict)
            {
                player.RestrictCamera();
            }
            else
            {
                player.FreeCamera();
            }
		}
	}
}
