using Godot;
using System;
using System.Linq;

public partial class SoundPlayer : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public static SoundPlayer Instance { get; private set; }
	
	public void Play(AudioStream audioStream, int volume = 0, bool randomPitch = false) {
		if (audioStream == null) { return; }

		foreach (var player in GetChildren().Cast<AudioStreamPlayer2D>()) {
			if (!player.Playing) {
				player.VolumeDb = volume;
				player.Stream = audioStream;
				player.Play();
				player.PitchScale = randomPitch ? GD.Randf() * (1.2f - 0.8f) + 0.8f : 1f;
			}
			break;
		}
	}
}
