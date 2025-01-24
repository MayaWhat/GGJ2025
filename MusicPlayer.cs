using Godot;

public partial class MusicPlayer : AudioStreamPlayer
{
    AudioStreamPlaybackInteractive playback;

    public override void _Ready()
    {
        base._Ready();
        playback = Stream.InstantiatePlayback() as AudioStreamPlaybackInteractive;
    }

    public void PlayClip(string clip)
    {
        playback.SwitchToClipByName(clip);
        if (!Playing)
        {
            Play();
        }
    }
}
