using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;

public class PickUpAction : GoapAction<string, object>
{

    // TODO: PASS INVENTORY TO CONSTRUCTOR
    public PickUpAction() : base("PickUpAction") { }
    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        return base.CheckProceduralCondition(stackData) && stackData.settings.HasKey("keyPosition");
    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        if (stackData.settings.TryGetValue("itemPosition", out var workstationPosition))
            preconditions.Set("isAtPosition", workstationPosition);
        return preconditions;
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Set("hasKey", true);
        return base.GetEffects(stackData);
    }

}
