using Godot;

public class HasItem : GoapGoalAdvanced<string, object> {

    public HasItem(string itemName) {
        Name = "HasItem" + itemName;
        goal.Set("hasItem" + itemName, true);
    }
}