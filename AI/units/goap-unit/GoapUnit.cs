using Godot;
using ReGoap.Core;
using System.Collections.Generic;

public class GoapUnit : KinematicBody2D
{

    public Agent GoapAgent;
    protected List<IReGoapAction<string, object>> availableActions;

    protected List<IReGoapGoal<string, object>> availableGoals;

    protected AgentMemory agentMemory;


    protected MoveToPointSystem moveToPointSystem = new MoveToPointSystem();
    public override void _Ready()
    {
        availableActions = new List<IReGoapAction<string, object>>();
        foreach(var actionNode in GetNode("AI/Actions").GetChildren()) {
            availableActions.Add(((IAgentAction)actionNode).GoapAction);
        }
        availableActions.Add(new GoToAction(this.moveToPointSystem));
        availableGoals = new List<IReGoapGoal<string, object>>();

        agentMemory = GetNode<AgentMemory>("AI/AgentMemory");
        GoapAgent = new Agent("GoapUnit", agentMemory.Memory, availableActions, availableGoals);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (moveToPointSystem.isActive)
        {
            moveToPointSystem.Move(this, 100);
        }

        foreach (var goal in availableGoals) {
            if (goal is GoapGoalAdvanced<string, object>) {
                ((GoapGoalAdvanced<string, object>)goal).Update();
            }
        }
    }

    public void setGoals(List<IReGoapGoal<string, object>> goals)
    {
        availableGoals = goals;
        // GD.Print("SET_GOAL " + goal.Name);
        // var newGals = new List<IReGoapGoal<string, object>>();
        // newGals.Add(goal);
        GoapAgent.SetGoalsSet(availableGoals);
    }

}
