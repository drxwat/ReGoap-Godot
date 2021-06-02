
using Godot;
public class AgentBagSensor : Node, IAgentSensor
{
    [Export]
    private NodePath bagNode;

    private GoapSensor<string, object> _sensor; 
    public GoapSensor<string, object> Sensor {
        get => _sensor;
        set => _sensor = value;
    }

    public override void _Ready()
    {
        var bag = GetNode<Bag>(bagNode);
        if (bag == null)
        {
            GD.PushError("AgentBagSensor didn't find Bag node. Agent Bag wont' work.");
            return;
        }
        Sensor = new BagSensor(bag);

    }

}


public class BagSensor : GoapSensor<string, object>
{
    protected Bag bag;

    public BagSensor(Bag _bag) {
        bag = _bag;
    }

    public override void UpdateSensor()
    {
        var state = memory.GetWorldState();
        foreach (var pair in bag.GetItems())
        {
            if (pair.Value > 0)
            {
                state.Set("hasItem" + pair.Key, true);
            }
            else
            {
                state.Remove("hasItem" + pair.Key);
                bag.ClearItem(pair.Key);
            }
        }
    }
}