using Godot;
using System.Linq;

public class AgentMemory : Node
{
    public GoapMemory<string, object> Memory = new GoapMemory<string, object>();
    private IAgentSensor[] sensors;

    public ulong SensorsUpdateDelay = 3;
    private ulong sensorsUpdateCooldown = OS.GetUnixTime();

    public override void _Ready()
    {
        sensors = GetChildren().OfType<IAgentSensor>().ToArray();
        foreach (var sensorNode in sensors)
        {
            sensorNode.Sensor.Init(Memory);
            sensorNode.Sensor.UpdateSensor();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (OS.GetUnixTime() > sensorsUpdateCooldown)
        {
            sensorsUpdateCooldown = OS.GetUnixTime();
            foreach (var sensorNode in sensors)
            {
                sensorNode.Sensor.UpdateSensor();
            }
        }
    }

}