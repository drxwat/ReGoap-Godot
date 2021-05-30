using Godot;
using System;

public class GOAP : Node2D
{
    private GoapUnit unit;
    private Position2D targetPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        unit = GetNode<GoapUnit>("GoapUnit");
        targetPosition = GetNode<Position2D>("Position2D");

        unit.setGoal(new MoveToGoal(targetPosition.GlobalPosition));
    }

}
