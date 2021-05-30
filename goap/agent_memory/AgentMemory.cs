using ReGoap.Core;
using Godot;
using System.Linq;

public class AgentMemory : Node
{
    public GoapMemory<string, object> Memory = new GoapMemory<string, object>();
    private IReGoapSensor<string, object>[] sensors;

    public float SensorsUpdateDelay = 0.3f;
    private float sensorsUpdateCooldown;

    public override void _Ready()
    {
        sensors = GetChildren().OfType<IReGoapSensor<string, object>>().ToArray(); // TODO: change to Sensor Node type
        foreach (var sensor in sensors)
        {
            sensor.Init(Memory);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (OS.GetUnixTime() > sensorsUpdateCooldown)
        {
            sensorsUpdateCooldown = OS.GetUnixTime() + SensorsUpdateDelay;

            foreach (var sensor in sensors)
            {
                sensor.UpdateSensor();
            }
        }
    }

}