using Godot;
using System;
using System.Collections.Generic;

[Tool]
public class ItemsVision : Area2D
{
    [Export]
    public float Radius = 250.0f;

    private List<Action<Item>> subscribersOnAdd = new List<Action<Item>>();
    private List<Action<Item>> subscribersOnDelete = new List<Action<Item>>();

    private List<Action<ItemOpenable>> subscribersOnOpen = new List<Action<ItemOpenable>>();


    private List<Item> knownItems = new List<Item>();

    public override void _Ready()
    {
        CollisionShape2D shape = GetNode<CollisionShape2D>("CollisionShape2D");
        CircleShape2D circle = new CircleShape2D();
        circle.Radius = Radius;
        shape.Shape = circle;

        GD.Print(GetOverlappingBodies());
    }

    public void OnItemDetect(Item item)
    {
        item.Connect("Deleted", this, "OnItemDeleted", new Godot.Collections.Array { item });
        knownItems.Add(item);
        foreach (var onAdd in subscribersOnAdd)
        {
            onAdd(item);
        }
        if (item.GetType().Equals(typeof(ItemOpenable)))
        {
            item.Connect("Opened", this, "OnItemOpened", new Godot.Collections.Array { item });
        }
    }

    public void Subscribe(Action<Item> onItemAdd, Action<Item> onItemDelete, Action<ItemOpenable> onItemOpen)
    {
        foreach (var item in knownItems)
        {
            onItemAdd(item);
        }
        subscribersOnAdd.Add(onItemAdd);
        subscribersOnDelete.Add(onItemDelete);
        subscribersOnOpen.Add(onItemOpen);
    }

    public void OnItemOpened(ItemOpenable item)
    {
        foreach (var onOpen in subscribersOnOpen)
        {
            onOpen(item);
        }
    }

    public void OnItemDeleted(Item item)
    {
        foreach (var onDelete in subscribersOnDelete)
        {
            onDelete(item);
        }
        item.Disconnect("Deleted", this, "OnItemDeleted");
        if (item.GetType().Equals(typeof(ItemOpenable)))
        {
            item.Disconnect("Opened", this, "OnItemOpened");
        }
        knownItems.Remove(item);
    }

    public void UnSubscribe(Action<Item> onItemAdd, Action<Item> onItemDelete)
    {
        subscribersOnAdd.Remove(onItemAdd);
        subscribersOnDelete.Remove(onItemDelete);
    }
}
