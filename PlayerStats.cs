using System;
using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export] public int HighScore { get; private set; }
    [Export] public int CurrentScore { get; private set; }

    public TimeSpan CurrentTime { get; private set;}

    [Signal] public delegate void ScoreChangedEventHandler();

    [Signal] public delegate void TimerChangedEventHandler();

    [Signal] public delegate void EndedEventHandler();

    public void UpdateHighScore(int score)
    {
        HighScore = score;
        EmitSignal(SignalName.ScoreChanged);
    }

    public void UpdateCurrentScore(int score)
    {
        CurrentScore = score;
        EmitSignal(SignalName.ScoreChanged);
    }

    public void UpdateTimer(TimeSpan currentTime)
    {
        CurrentTime = currentTime;
        EmitSignal(SignalName.TimerChanged);
    }

    public void End()
    {
        EmitSignal(SignalName.Ended);
    }
}