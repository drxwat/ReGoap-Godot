using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;
using System.Linq;

public class PickUpItemAction : Node, IAgentAction
{
    [Export]
    private NodePath bagNode;

    private GoapAction<string, object> _goapAction;

    public GoapAction<string, object> GoapAction
    {
        get => _goapAction;
        set => _goapAction = value;
    }
    public override void _Ready()
    {
        if (bagNode != null) {
            GoapAction = new PickUpItemGoapAction(GetNode<Bag>(bagNode));
        } else {
            GD.PrintErr("BagNode is not set for PickUpItemAction. PickUp behavior won't work");
        }
    }
}

// TODO: Add all resources to settings in loop and implement cost function to give planner metric to chose
public class PickUpItemGoapAction : GoapAction<string, object>
{

    protected Bag bag;

    public PickUpItemGoapAction(Bag _bag) : base("PickUpItemAction") { 
        bag = _bag;
    }


    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);

        if (settings.TryGetValue("pickUpItem", out var _item)) {
            var item = ((Item)_item);
            bag.AddItem(item.ItemName, 1);
            item.Delete();
            doneCallback(this);
        } else {
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