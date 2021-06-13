using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    private HashSet<ItemsEnum> openableItems;
    public OpenItemGoapAction(HashSet<ItemsEnum> _openableItems) : base("OpenItemAction")
    {
        openableItems = _openableItems;
    }

    public async override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);

        if (settings.TryGetValue("openItem", out var _item))
        {
            var item = ((ItemOpenable)_item);
            var isSuccess =  await item.Open();
            if (isSuccess) {
                doneCallback(this);
            } else {
                failCallback(this);
            }
        }
        else
        {
            failCallback(this);
        }
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        return base.CheckProceduralCondition(stackData) && 
            stackData.settings.HasKey("openItem") && 
            stackData.settings.HasKey("dropItem");
    }

    public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
    {
        preconditions.Clear();
        if (stackData.settings.TryGetValue("openItem", out var openItem))
        {
            preconditions.Set("isAtPosition", ((ItemOpenable)openItem).GlobalPosition);
        }
        return preconditions;
    }

    public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
    {
        effects.Clear();
        if (stackData.settings.TryGetValue("dropItem", out var itemName))
        {
            effects.Set("hasItem" + (string)itemName, true);
        }
        return base.GetEffects(stackData);
    }

    public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
    {
        settings.Clear();
        var neededItemName = getNeededItemFromGoal(stackData.goalState);
        if (neededItemName == null) {
            return base.GetSettings(stackData);
        }

        var neededItemType = ItemHelper.GetItemTypeByName(neededItemName);
        foreach (var openableType in openableItems)
        {
            // Iterating over all types of openable items
            var openables = (List<Item>)stackData.currentState.Get(ItemHelper.GetItemNameByType(openableType));
            if (openables == null) {
                continue;
            }
            foreach (var openable in openables)
            {
                ItemOpenable openableItem = (ItemOpenable)openable;
                if (openableItem.isOpened)
                {
                    continue;
                }
                if (openableItem.possibleDrop.Contains(neededItemType))
                {
                    // TODO: Set all openable items that drop needed item
                    settings.Set("openItem", openableItem);
                    settings.Set("dropItem", neededItemName);
                }
            }
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