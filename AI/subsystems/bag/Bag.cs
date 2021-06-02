using System.Collections.Generic;
using Godot;
public class Bag : Node // TODO: Change implementaton from polling to reactive. See ItemLocator
{
    private Dictionary<string, float> items = new Dictionary<string, float>();

    public void AddItem(string itemName, float amount)
    {
        if (!items.ContainsKey(itemName))
            items[itemName] = 0;
        items[itemName] += amount;
    }

    public float GetItem(string itemName)
    {
        var amount = 0f;
        items.TryGetValue(itemName, out amount);
        return amount;
    }

    public Dictionary<string, float> GetItems()
    {
        return items;
    }

    public void RemoveItem(string itemName, float amount)
    {
        items[itemName] -= amount;
    }

    // We need this method to be called by BagSensor when it updates memory state
    public void ClearItem(string itemName)
    {
        items.Remove(itemName);
    }

}