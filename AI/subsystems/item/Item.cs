using Godot;
using System;

public static class ItemHelper {
    public static string GetItemNameByType(ItemsEnum type) {
        return Enum.GetName(typeof(ItemsEnum), type);
    }

    public static ItemsEnum GetItemTypeByName(string name) {
        return (ItemsEnum)Enum.Parse(typeof(ItemsEnum), name);
    }
}

public class Item : KinematicBody2D
{

    [Signal]
    protected delegate void Deleted();


    [Export]
    public ItemsEnum itemType = ItemsEnum.None;

    public bool isPickable = true;

    public override void _Ready()
    {
        if (itemType == ItemsEnum.None) {
            GD.PrintErr("{} has None type of item", this);
        }
    }

    public string ItemName{
        get { return ItemHelper.GetItemNameByType(itemType); }
    }

    public virtual void Delete()
    {
        EmitSignal(nameof(Deleted));
        QueueFree();
    }
}