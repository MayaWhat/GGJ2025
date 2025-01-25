using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class MusicPlayer : AudioStreamPlayer
{
    public readonly static Dictionary<string, (string Bubble, string Popped)> Clips = new()
    {
        {"Water", ("Water1", "Water2")},
        {"Sky", ("Sky1", "Sky2")},
    };

    public string Current { get; set; } = Clips.Keys.First();

    AudioStreamPlaybackInteractive GetInteractiveStreamPlayback() => GetStreamPlayback() as AudioStreamPlaybackInteractive;

    void PlayClip(string clip)
    {
        if (!Playing)
        {
            Play();
        }
        GetInteractiveStreamPlayback().SwitchToClipByName(clip);
    }

    public void Bubble()
    {
        PlayClip(Clips[Current].Bubble);
    }

    public void Popped()
    {
        PlayClip(Clips[Current].Popped);
    }
}
