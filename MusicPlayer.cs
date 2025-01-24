using Godot;

public partial class MusicPlayer : AudioStreamPlayer
{
    AudioStreamPlaybackInteractive GetInteractiveStreamPlayback() => GetStreamPlayback() as AudioStreamPlaybackInteractive;

    public void PlayClip(string clip)
    {
        if (!Playing)
        {
            Play();
        }
        GetInteractiveStreamPlayback().SwitchToClipByName(clip);
    }
}
