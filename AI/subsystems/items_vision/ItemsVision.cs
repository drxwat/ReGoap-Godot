using Godot;
using System;
using System.Collections.Generic;

public class ItemsVision : Node2D
{

    private List<Action<string, Vector2>> subscribers;

    private List<Node2D> knownItems; // TODO: Change to ITEM base class
    // TODO: Subscribe On Item Deletion

    public void OnItemDetect(Node2D body) // TODO: Change to ITEM base class
    {
        knownItems.Add(body);
        foreach (var subscriber in subscribers)
        {
            subscriber("ITEM_NAME", body.GlobalPosition);
        }
    }

    public void Subscribe(Action<string, Vector2> callback) // TODO: Add callback on item removal
    {
        foreach (var item in knownItems)
        {
            callback("ITEM_NAME", item.GlobalPosition);
        }
        subscribers.Add(callback);
    }

    public void UnSubscribe(Action<string, Vector2> callback)
    {
        subscribers.Remove(callback);
    }
}
