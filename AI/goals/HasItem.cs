using Godot;

public class HasItem : GoapGoalAdvanced<string, object> { 

    public HasItem(string itemName) { // TODO: Replace string with enum 
        Name = "HasItem" + itemName;
        goal.Set("hasItem" + itemName, true);
    }
}