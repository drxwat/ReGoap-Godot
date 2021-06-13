using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ItemOpenable : Item
{

    [Signal]
    protected delegate void Opened();

    [Export]
    public Godot.Collections.Array<ItemsEnum> possibleDrop = new Godot.Collections.Array<ItemsEnum> { };

    [Export]
    public Godot.Collections.Array<ItemsEnum> drop = new Godot.Collections.Array<ItemsEnum> { };

    [Export]
    public bool isRandomDrop = false;

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
        aIGlobals.openableItems.Add(this.itemType);
        HashSet<ItemsEnum> dropSet = new HashSet<ItemsEnum>(drop);
        dropSet.ExceptWith(possibleDrop);
        if (dropSet.Count > 0)
        {
            GD.PrintErr("ItemOpenable {} contains imposible drop ids {}", this.Name, dropSet);
        }
    }

    public async Task<bool> Open()
    {
        GD.Print("OPENNING ITEM");
        if (this.isOpened)
        {
            return false;
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
        await Task.Delay(500);
        return true;
    }

}
