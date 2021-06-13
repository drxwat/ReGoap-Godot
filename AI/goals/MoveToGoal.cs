using Godot;

public class MoveToGoal : GoapGoalAdvanced<string, object> {

    public MoveToGoal(Vector2 targetPosition) {
        Name = "MoveToGoal point: " + targetPosition.ToString();
        goal.Set("isAtPosition", targetPosition);
    }
}