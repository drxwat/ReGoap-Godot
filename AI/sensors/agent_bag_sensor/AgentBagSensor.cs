
using Godot;
public class AgentBagSensor : Node, IAgentSensor
{
    [Export]
    private NodePath bagNode;

    private Bag bag;

    private GoapSensor<string, object> _sensor;
    public GoapSensor<string, object> Sensor
    {
        get => _sensor;
        set => _sensor = value;
    }

    public override void _Ready()
    {
        bag = GetNode<Bag>(bagNode);
        if (bag == null)
        {
            GD.PushError("AgentBagSensor didn't find Bag node. Agent Bag wont' work.");
            return;
        }
        Sensor = new BagSensor();
        bag.Connect("ItemAdded", this, "OnItemAdd");
        bag.Connect("ItemRemoved", this, "onItemDelete");
    }

    public void OnItemAdd(ItemsEnum itemType)
    {
        ((BagSensor)Sensor).OnItemAdd(itemType);
    }

    public void onItemDelete(ItemsEnum itemType)
    {
        if (bag.GetItem(itemType) <= 0)
        {
            ((BagSensor)Sensor).onItemDelete(itemType);
        }
    }
}


public class BagSensor : GoapSensor<string, object>
{

    public void OnItemAdd(ItemsEnum itemType)
    {
        GD.Print("Added Item To State: ", ItemHelper.GetItemNameByType(itemType));
        var state = memory.GetWorldState();
        state.Set("hasItem" + ItemHelper.GetItemNameByType(itemType), true);
    }

    public void onItemDelete(ItemsEnum itemType)
    {
        GD.Print("Removed Item To State: ", ItemHelper.GetItemNameByType(itemType));
        var state = memory.GetWorldState();
        state.Remove("hasItem" + ItemHelper.GetItemNameByType(itemType));
    }

}