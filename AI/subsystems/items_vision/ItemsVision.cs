using Godot;
using System;
using System.Collections.Generic;

public class ItemsVision : Node2D
{
    [Export]
    public float Radius = 50.0f;

    private List<Action<Item>> subscribersOnAdd = new List<Action<Item>>();
    private List<Action<Item>> subscribersOnDelete = new List<Action<Item>>();


    private List<Item> knownItems = new List<Item>();
    // TODO: Subscribe On Item Deletion

    public void OnItemDetect(Item item)
    {

        item.Connect("Deleted", this, "OnItemDeleted", new Godot.Collections.Array { item });
        knownItems.Add(item);
        foreach (var onAdd in subscribersOnAdd)
        {
            onAdd(item);
        }
    }

    public void Subscribe(Action<Item> onItemAdd, Action<Item> onItemDelete)
    {
        foreach (var item in knownItems)
        {
            onItemAdd(item);
        }
        subscribersOnAdd.Add(onItemAdd);
        subscribersOnDelete.Add(onItemDelete);
    }

    public void OnItemDeleted(Item item)
    {
        foreach (var onDelete in subscribersOnDelete)
        {
            onDelete(item);
        }
        knownItems.Remove(item);
    }

    public void UnSubscribe(Action<Item> onItemAdd, Action<Item> onItemDelete)
    {
        subscribersOnAdd.Remove(onItemAdd);
        subscribersOnDelete.Remove(onItemDelete);
    }
}
