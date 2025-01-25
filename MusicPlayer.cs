using System.Collections.Generic;
using Godot;

public partial class MusicPlayer : AudioStreamPlayer
{
    public const string PoppedClip = "Popped";
    public readonly string[] LevelClips = [
        "Water",
        "Sky",
        "Space"
    ];

    public readonly static Dictionary<string, (string Bubble, string Popped)> Clips = new()
    {
        {"Water", ("Water1", "Water2")},
        {"Sky", ("Sky1", "Sky2")},
    };

    public int Current { get; set; } = 0;
    private bool _popped = false;

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
        _popped = false;
        PlayClip(LevelClips[Current]);
    }

    public void Popped()
    {
        _popped = true;
        PlayClip(PoppedClip);
    }

    public void ChangeLevel(int level)
    {
        if (level == Current)
        {
            return;
        }

        Current = level;
        if (!_popped)
        {
            PlayClip(LevelClips[Current]);
        }
    }
}
