
using Godot;
public class AgentBagSensor : AgentSensor
{
    [Export]
    private NodePath bagNode;

    new public BagSensor Sensor = new BagSensor();

    public override void _Ready()
    {
        var bag = GetNode<Bag>(bagNode);
        if (bag == null)
        {
            GD.PushWarning("AgentBagSensor didn't find Bag node. Agent Bag wont' work.");
            return;
        }
        Sensor.Bag = bag;
    }

}


public class BagSensor : GoapSensor<string, object>
{
    public Bag Bag;

    public override void UpdateSensor()
    {
        if (Bag == null)
        {
            return;
        }
        var state = memory.GetWorldState();
        foreach (var pair in Bag.GetItems())
        {
            if (pair.Value > 0)
            {
                state.Set("hasResource" + pair.Key, true);
            }
            else
            {
                state.Remove("hasResource" + pair.Key);
                Bag.ClearItem(pair.Key);
            }
        }
    }
}