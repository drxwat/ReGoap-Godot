using Godot;
using ReGoap.Core;
using System.Collections.Generic;

public class GoapUnit : KinematicBody2D
{

    protected GoapAgent<string, object> goapAgent;
    protected List<IReGoapAction<string, object>> availableActions;

    protected List<IReGoapGoal<string, object>> availableGoals;

    protected MoveToPointSystem moveToPointSystem = new MoveToPointSystem();
    public override void _Ready()
    {
        GD.Print("Unit Created");
        availableActions = new List<IReGoapAction<string, object>>(); // #TODO: fill list with actions subclasses
        availableActions.Add(new GoToAction(this.moveToPointSystem));
        availableGoals = new List<IReGoapGoal<string, object>>(); // #TODO: fill list with goals subclasses

        var goapAgentMemory = new GoapMemory<string, object>();
        goapAgent = new Agent("GoapUnit" ,goapAgentMemory, availableActions, availableGoals);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (moveToPointSystem.isActive) {
            moveToPointSystem.Move(this, 30);
        }
    }

    public void setGoal(GoapGoal<string, object> goal) {
        GD.Print("SET GOAL GO_TO");
        var newGals = new List<IReGoapGoal<string, object>>();
        newGals.Add(goal);
        goapAgent.SetGoalsSet(newGals);
    }

}
