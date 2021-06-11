using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;
using System.Linq;

public class OpenItemAction : Node, IAgentAction
{
    private GoapAction<string, object> _goapAction;

    public GoapAction<string, object> GoapAction
    {
        get => _goapAction;
        set => _goapAction = value;
    }

    private static AIGlobals aIGlobals;

    public override void _Ready()
    {
        aIGlobals = GetNode<AIGlobals>("/root/AiGlobals");
        if (aIGlobals == null)
        {
            GD.PrintErr("/root/AiGlobals is not found");
        }
        GoapAction = new OpenItemGoapAction(aIGlobals.openableItems);
    }

}

public class OpenItemGoapAction : GoapAction<string, object>
{

    private HashSet<string> openableItems;
    public OpenItemGoapAction(HashSet<string> _openableItems) : base("OpenItemAction")
    {
        openableItems = _openableItems;
    }

    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);

        if (settings.TryGetValue("itemToOpen", out var _item))
        {
            // var item = ((Item)_item);
            // bag.AddItem(item.ItemName, 1);
            // item.Delete();
            doneCallback(this);
        }
        else
        {
            failCallback(this);
        }
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        return base.CheckProceduralCondition(stackData) && stackData.settings.HasKey("pickUpItem");
    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        preconditions.Clear();
        if (stackData.settings.TryGetValue("pickUpItem", out var item))
        {
            preconditions.Set("isAtPosition", ((Item)item).GlobalPosition);
        }
        return preconditions;
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Clear();
        if (stackData.settings.TryGetValue("pickUpItem", out var item))
        {
            effects.Set("hasItem" + ((Item)item).ItemName, true);
        }
        return base.GetEffects(stackData);
    }

    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        settings.Clear();
        var neededItemName = getNeededItemFromGoal(stackData.goalState);
        foreach (var openableName in openableItems)
        {
            var openables = (List<ItemOpenable>)stackData.currentState.Get(openableName);
            foreach (var openable in openables)
            {
                if (openable.isOpened)
                {
                    continue;
                }
                if (openable.possibleDrop.Contains(neededItemName))
                {
                    // TODO: Set all openable items that drop needed item
                }
            }
        }

        if (neededItemName == null || !stackData.currentState.HasKey(neededItemName))
        {
            return base.GetSettings(stackData);
        }

        // Getting known items list from world state
        var items = (List<Item>)stackData.currentState.Get(neededItemName);
        Vector2 agentPosition = Vector2.Zero;

        // TODO: Find Clothest item
        if (items.Count() > 0)
        {
            settings.Set("pickUpItem", items[0]);
        }

        return base.GetSettings(stackData);
    }

    protected virtual string getNeededItemFromGoal(ReGoapState<string, object> goalState)
    {
        foreach (var pair in goalState.GetValues())
        {
            if (pair.Key.StartsWith("hasItem"))
            {
                return pair.Key.Substring(7);
            }
        }
        return null;
    }
}