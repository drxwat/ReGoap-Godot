using Godot;

public class MoveToGoal : GoapGoal<string, object> {

    public MoveToGoal(Vector2 targetPosition) {
        Name = "MoveToGoal point: " + targetPosition.ToString();
        goal.Set("isAtPosition", targetPosition);
    }
}