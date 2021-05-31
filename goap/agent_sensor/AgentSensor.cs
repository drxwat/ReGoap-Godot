using Godot;

// Base node class for all agent sensors.
// Inherit it and overwrite Sensor property with your own sensor class
public class AgentSensor : Node
{
    public GoapSensor<string, object> Sensor = new GoapSensor<string, object>();
}