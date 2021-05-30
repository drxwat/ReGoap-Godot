using Godot;
using System;
using ReGoap.Core;

public class MoveToPointSystem {

    public bool isActive = false;
    protected float tolleranceDistance = 5.0F; 

    private Vector2 targetPosition;
    private Action onDoneCallback;

    public void Activate(Vector2 target, Action onDoneMovement) {
        isActive = true;
        targetPosition = target;
        onDoneCallback = onDoneMovement;
    }

    public void Move(KinematicBody2D agentBody, float speed) {
        if (agentBody.GlobalPosition.DistanceTo(targetPosition) <= tolleranceDistance) {
            isActive = false;
            onDoneCallback();
            return;
        }
        var direction = agentBody.GlobalPosition.DirectionTo(targetPosition);
        agentBody.MoveAndSlide(direction * speed);
    }
}