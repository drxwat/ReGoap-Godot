using Godot;

public class AgentLocationSensor : Node2D, IAgentSensor
{
    private GoapSensor<string, object> _sensor;
    public GoapSensor<string, object> Sensor
    {
        get => _sensor;
        set => _sensor = value;
    }

    public override void _Ready()
    {
        Sensor = new LocationSensor(this);
    }
}


public class LocationSensor : GoapSensor<string, object>
{

    Node2D locationNode;

    public LocationSensor(Node2D _locationNode)
    {
        locationNode = _locationNode;
    }

    public override void UpdateSensor()
    {
        var state = memory.GetWorldState();
        state.Set("isAtPosition", locationNode.Position);
    }
}