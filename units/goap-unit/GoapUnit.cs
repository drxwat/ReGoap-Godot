using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;

public class GoapUnit : KinematicBody2D
{

    protected GoapAgent<string, object> goapAgent;
    protected List<IReGoapAction<string, object>> availableActions;

    protected List<IReGoapGoal<string, object>> availableGoals;
    public override void _Ready()
    {
        availableActions = new List<IReGoapAction<string, object>>(); // #TODO: fill list with actions subclasses
        availableGoals = new List<IReGoapGoal<string, object>>(); // #TODO: fill list with goals subclasses

        var goapAgentMemory = new GoapMemory<string, object>();
        goapAgent = new Agent(goapAgentMemory, availableActions, availableGoals);
    }

}
