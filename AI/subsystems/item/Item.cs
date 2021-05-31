using Godot;

public class Item : Node2D
{
    public void Remove()
    {
        QueueFree();
    }
}