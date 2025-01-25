using Godot;

public partial class FadeOutOnPlayerHeight : Node2D
{
    [Export]
    private Player _player;

    [Export]
    private int _bottom;

    [Export]
    private int _top;

    [Export]
    private float _threshold;

    [Export]
    private Node2D[] _applyTo;

    public override void _Process(double delta)
    {
        var diff = _threshold;

        if (_player.GlobalPosition.Y > _bottom - _threshold)
        {
            diff = 1f - Mathf.Clamp(_player.GlobalPosition.Y - _bottom, 0f, _threshold);
        }
        else if (_player.GlobalPosition.Y < _top + _threshold)
        {
            diff = 1f - Mathf.Clamp(_top - _player.GlobalPosition.Y, 0f, _threshold);
        }

        var percent = Mathf.Clamp(diff / _threshold, 0f, 1f);
        foreach (var node in _applyTo)
        {
            node.Modulate = new Color(node.Modulate.R, node.Modulate.G, node.Modulate.B, percent);
        }
        
    }
}
