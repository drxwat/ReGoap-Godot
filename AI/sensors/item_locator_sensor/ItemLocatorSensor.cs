using Godot;
using System;
using System.Collections.Generic;

public class ItemLocatorSensor : AgentSensor
{
    [Export]
    private NodePath itemsVisionNode;

    new public ItemLocatorGoapSensor Sensor = new ItemLocatorGoapSensor();

    public override void _Ready()
    {
        var itemsVision = GetNode<ItemsVision>(itemsVisionNode);
        if (itemsVision == null)
        {
            GD.PushWarning("ItemLocatorSensor didn't find ItemsVision node. ItemLocatorSensor wont' work.");
            return;
        }
        Sensor.ItemsVision = itemsVision;
    }
}

public class ItemLocatorGoapSensor : GoapSensor<string, object>
{

    public ItemsVision ItemsVision;

    public void EnableSensor()
    {
        if (ItemsVision == null)
        {
            GD.PushWarning("Cant' enable ItemLocatorGoapSensor. ItemsVision is not set");
            return;
        }
        ItemsVision.Subscribe(this.OnItemDetected, this.OnItemDeleted);
    }

    public void DisableSensor()
    {
        if (ItemsVision == null)
        {
            GD.PushWarning("Cant' disable ItemLocatorGoapSensor. ItemsVision is not set");
            return;
        }
        ItemsVision.UnSubscribe(this.OnItemDetected, this.OnItemDeleted);
    }

    private void OnItemDetected(Item item)
    {
        var state = memory.GetWorldState();

        if (state.TryGetValue(item.ItemName, out var _items))
        {
            var items = (List<Item>)_items;
            items.Add(item);
        }
        else
        {
            state.Set(item.ItemName, new List<Item> { item });
        }
    }

    private void OnItemDeleted(Item item)
    {
        var state = memory.GetWorldState();
        if (state.TryGetValue(item.ItemName, out var _items))
        {
            var items = (List<Item>)_items;
            items.Remove(item);
        }
    }
}