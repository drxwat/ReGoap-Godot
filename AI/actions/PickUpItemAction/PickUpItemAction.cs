using Godot;
using ReGoap.Core;
using System.Collections.Generic;

public class PickUpItemAction : Node
{

}

public class PickUpItemGoapAction : GoapAction<string, object>
{
    public PickUpItemGoapAction() : base("PickUpItemAction") { }


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

    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        settings.Clear();
        var neededItemName = getNeededItemFromGoal(stackData.goalState);
        if (neededItemName == null || !stackData.currentState.HasKey(neededItemName))
        {
            return base.GetSettings(stackData);
        }

        var items = (List<Item>)stackData.currentState.Get(neededItemName);


        return base.GetSettings(stackData);
    }

    protected virtual string getNeededItemFromGoal(ReGoapState<string, object> goalState)
    {
        foreach (var pair in goalState.GetValues())
        {
            if (pair.Key.StartsWith("hasItem"))
            {
                return pair.Key.Substring(11);
            }
        }
        return null;
    }

}