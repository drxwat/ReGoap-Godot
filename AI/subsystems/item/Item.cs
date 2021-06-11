using Godot;

public class Item : KinematicBody2D
{

    [Signal]
    protected delegate void Deleted();

    [Export]
    public string ItemName = "UnknownItem";

    public bool isPickable = true;

    public virtual void Delete()
    {
        EmitSignal(nameof(Deleted));
        QueueFree();
    }
}