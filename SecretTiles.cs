using Godot;

public partial class SecretTiles : TileMapLayer
{
    bool revealed = false;

    [Export]
    AnimationPlayer _animationPlayer;

    public void Reveal()
    {
        if (revealed)
        {
            return;
        }

        revealed = true;
        _animationPlayer.Play("Reveal");
    }
}
