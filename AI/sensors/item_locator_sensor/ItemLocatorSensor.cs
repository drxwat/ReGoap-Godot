using Godot;
using System.Collections.Generic;

public class ItemLocatorSensor : Node, IAgentSensor
{
    [Export]
    private NodePath itemsVisionNode;

    private GoapSensor<string, object> _sensor;
    public GoapSensor<string, object> Sensor
    {
        get => _sensor;
        set => _sensor = value;
    }

    public override void _Ready()
    {
        var itemsVision = GetNode<ItemsVision>(itemsVisionNode);
        if (itemsVision == null)
        {
            GD.PushError("ItemLocatorSensor didn't find ItemsVision node. ItemLocatorSensor wont' work.");
            return;
        }
        Sensor = new ItemLocatorGoapSensor(itemsVision);

    }
}

public class ItemLocatorGoapSensor : GoapSensor<string, object>
{

    protected ItemsVision itemsVision;

    public ItemLocatorGoapSensor(ItemsVision _itemsVision)
    {
        itemsVision = _itemsVision;
        EnableSensor();
    }

    public void EnableSensor()
    {
        itemsVision.Subscribe(this.OnItemDetected, this.OnItemDeleted, this.OnItemOpened);
    }

    public void DisableSensor()
    {
        itemsVision.UnSubscribe(this.OnItemDetected, this.OnItemDeleted);
    }

    private void OnItemOpened(ItemOpenable item)
    {
        // DEPENDS FROM HOW ITEM DROPS IT'S CONTENT THIS CALLBACK CAN BE HELPFULL
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
            if (items.Count <= 0) {
                state.Remove(item.ItemName);
            }
        }
    }
}