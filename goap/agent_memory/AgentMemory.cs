using Godot;
using System.Linq;

public class AgentMemory : Node
{
    public GoapMemory<string, object> Memory = new GoapMemory<string, object>();
    private AgentSensor[] sensors;

    public float SensorsUpdateDelay = 0.3f;
    private float sensorsUpdateCooldown;

    public override void _Ready()
    {
        sensors = GetChildren().OfType<AgentSensor>().ToArray();
        foreach (var sensorNode in sensors)
        {
            sensorNode.Sensor.Init(Memory);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (OS.GetUnixTime() > sensorsUpdateCooldown)
        {
            sensorsUpdateCooldown = OS.GetUnixTime() + SensorsUpdateDelay;

            foreach (var sensorNode in sensors)
            {
                sensorNode.Sensor.UpdateSensor();
            }
        }
    }

}