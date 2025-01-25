using Godot;

public partial class RandomBubble : Node2D
{
	[Export] public AudioStream _popSound;
	[Export] public float _speed;
	[Export] public float _scaleGrowSpeed;
	[Export] public float _alphaDecreaseSpeed;
	[Export] public int _timeToPop;

	private GpuParticles2D _popParticles;
	private Sprite2D _sprite;
	private bool _stop;

	private double _timeAlive;

	public override void _Ready()
	{
		GetNode<Area2D>("%Area2D").AreaEntered += OnAreaEntered;
		_popParticles = GetNode<GpuParticles2D>("%PopParticles");
		_sprite = GetNode<Sprite2D>("%Sprite2D");
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.BubbleMe();
			Pop();
		}
	}

	private void Pop()
	{
		SoundPlayer.Instance.Play(_popSound, randomPitch: true);
		_popParticles.Emitting = true;
		_sprite.Visible = false;
		_stop = true;

		GetTree().CreateTimer(3).Timeout += QueueFree;
	}

	public override void _Process(double delta)
	{
		if (_stop) { return; }
		GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + _speed * (float)delta);
		Scale = new Vector2(Scale.X + _scaleGrowSpeed * (float)delta, Scale.Y + _scaleGrowSpeed * (float)delta);
		_sprite.Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - _alphaDecreaseSpeed * (float)delta);
		_timeAlive += delta;

		if (_timeAlive > _timeToPop) { Pop(); }
	}
}
