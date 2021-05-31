using Godot;

public class ItemLocatorSensor : AgentSensor
{
    new public ItemLocatorGoapSensor Sensor = new ItemLocatorGoapSensor();
}

public class ItemLocatorGoapSensor : GoapSensor<string, object>
{

    public override void UpdateSensor()
    {

    }
}