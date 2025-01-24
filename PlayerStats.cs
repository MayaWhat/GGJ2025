using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export] public int HighScore { get; private set; }
    [Export] public int CurrentScore { get; private set; }

    [Signal] public delegate void ScoreChangedEventHandler();

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
}