using Godot;

public class Item : Node2D
{

    [Signal]
    delegate void Deleted();

    [Export]
    public string ItemName = "UnknownItem";

    public void Remove()
    {
        EmitSignal(nameof(Deleted));
        QueueFree();
    }
}