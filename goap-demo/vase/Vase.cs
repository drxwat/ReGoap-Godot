using Godot;

public class Vase : ItemOpenable
{
    protected override void OnOpened()
    {
        base.OnOpened();
        GetNode<AnimatedSprite>("AnimatedSprite").Animation = "broken";
    }

}
