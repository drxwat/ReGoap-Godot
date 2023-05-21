using System.Collections.Generic;
using Godot;
public class Bag : Node
{

    [Signal]
    protected delegate ItemsEnum ItemAdded();

    [Signal]
    protected delegate ItemsEnum ItemRemoved();

    private readonly Dictionary<ItemsEnum, float> items = new Dictionary<ItemsEnum, float>();

    public void AddItem(ItemsEnum itemType, float amount)
    {
        GD.Print("Item added to BAG " + ItemHelper.GetItemNameByType(itemType));
        if (!items.ContainsKey(itemType))
            items[itemType] = 0;
        items[itemType] += amount;
        EmitSignal(nameof(ItemAdded), itemType);
    }

    public float GetItem(ItemsEnum itemType)
    {
		items.TryGetValue(itemType, out float amount);
		return amount;
    }

	public Dictionary<ItemsEnum, float> GetItems() => items;

	public void RemoveItem(ItemsEnum itemType, float amount)
    {
        if (items[itemType] >= 0)
        {
            GD.Print("Item removed from BAG " + ItemHelper.GetItemNameByType(itemType));
            items[itemType] -= amount;
            EmitSignal(nameof(ItemRemoved), itemType);
        }
    }

}