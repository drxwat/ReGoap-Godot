using Godot;
using System.Collections.Generic;


public class ItemOpenable : Item
{

    [Signal]
    protected delegate void Opened();

    [Export]
    public string[] possibleDrop = new string[] { };

    [Export]
    public string[] drop = new string[] { };

    public bool isOpened = false;


    private static AIGlobals aIGlobals;

    private Vector2 dropMargin = new Vector2(20, 20);


    public ItemOpenable()
    {
        isPickable = false;
    }

    public override void _Ready()
    {
        aIGlobals = GetNode<AIGlobals>("/root/AiGlobals");
        if (aIGlobals == null)
        {
            GD.PrintErr("/root/AiGlobals is not found");
        }
        aIGlobals.openableItems.Add(this.Name);
        HashSet<string> dropSet = new HashSet<string>(drop);
        dropSet.ExceptWith(possibleDrop);
        if (dropSet.Count > 0)
        {
            GD.PrintErr("ItemOpenable {} contains imposible drop ids {}", this.Name, dropSet);
        }
    }

    public void Open()
    {
        if (this.isOpened)
        {
            return;
        }
        int dropI = 1;
        foreach (var itemKey in drop)
        {
            PackedScene itemScene;
            if (aIGlobals.availableItems.TryGetValue(itemKey, out itemScene))
            {
                Node2D itemNode = (Node2D)itemScene.Instance();
                itemNode.Position = new Vector2(dropMargin.x * dropI, dropMargin.y);
                this.AddChild(itemNode);
            }
            dropI++;
        }
        EmitSignal(nameof(Opened));
        isOpened = true;
    }

}
