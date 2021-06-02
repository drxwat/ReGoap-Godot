using Godot;

public class HasItem : GoapGoal<string, object> {

    public HasItem(string itemName) {
        Name = "HasItem" + itemName;
        goal.Set("hasItem" + itemName, true);
    }
}