using Godot;
using ReGoap.Core;
using System.Collections.Generic;

public class GOAP : Node2D
{
    private GoapUnit unit;
    private Position2D targetPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        unit = GetNode<GoapUnit>("GoapUnit");
        targetPosition = GetNode<Position2D>("Position2D");

        // var goals = new List<IReGoapGoal<string, object>>();
        // goals.Add(new MoveToGoal(targetPosition.GlobalPosition));
        // goals.Add(new HasItem("Key"));
        // unit.setGoals(goals);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("start"))
        {
            GD.Print("START!");
            var goals = new List<IReGoapGoal<string, object>>();
            goals.Add(new MoveToGoal(targetPosition.GlobalPosition));
            goals.Add(new HasItem("Key"));
            unit.setGoals(goals);
			unit.GoapAgent.ForceCalculateNewGoal();
        }
    }

}
