using System;
using Godot;
using Godot.Collections;

public partial class ObstaclePath : Node2D
{
    [Export] public Array<Marker2D> Path;
}