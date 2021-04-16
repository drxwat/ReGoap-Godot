using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;


public class GoToAction : GoapAction<string, object>
{
    // TOOD: PASS NAVIGATOR TO CONSTRUCTOR
    public GoToAction() : base("GoToAction") { }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        OnDoneMovement();
        // if (settings.TryGetValue("objectivePosition", out var v))
        //     smsGoto.GoTo((Vector3)v, OnDoneMovement, OnFailureMovement);
        // else
        //     failCallback(this);
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        return base.CheckProceduralCondition(stackData) && stackData.settings.HasKey("objectivePosition");
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        if (stackData.settings.TryGetValue("objectivePosition", out var objectivePosition))
        {
            effects.Set("isAtPosition", objectivePosition);
        }
        else
        {
            effects.Clear();
        }
        return base.GetEffects(stackData);
    }

    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        if (stackData.goalState.TryGetValue("isAtPosition", out var isAtPosition))
        {
            settings.Set("objectivePosition", isAtPosition);
            return base.GetSettings(stackData);
        }
        return new List<ReGoapState<string, object>>();
    }

    // if you want to calculate costs use a non-dynamic/generic goto action
    public override float GetCost(GoapActionStackData<string, object> stackData)
    {
        var distance = 0.0f;
        if (stackData.settings.TryGetValue("objectivePosition", out object objectivePosition)
            && stackData.currentState.TryGetValue("isAtPosition", out object isAtPosition))
        {
            if (objectivePosition is Vector3 p && isAtPosition is Vector3 r)
                distance = (p - r).Length();
        }
        return base.GetCost(stackData) + Cost + distance;
    }

    protected virtual void OnFailureMovement()
    {
        failCallback(this);
    }

    protected virtual void OnDoneMovement()
    {
        GD.Print("MOVEMENT DONE");
        doneCallback(this);
    }
}
