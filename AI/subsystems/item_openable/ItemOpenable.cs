using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

public class ItemOpenable : Item
{

    [Signal]
    protected delegate void Opened();

    // Possible drop means what agent knows about that item
    [Export]
    public Godot.Collections.Array<ItemsEnum> possibleDrop = new Godot.Collections.Array<ItemsEnum> { };

    [Export]
    public ItemsEnum openableWithItem = ItemsEnum.None;

    // Actual drop that item will produce. Do not work if isRandomDrop=true
    [Export]
    public Godot.Collections.Array<ItemsEnum> drop = new Godot.Collections.Array<ItemsEnum> { };

    [Export]
    public bool isRandomDrop = false;

    // Probability distribusion applied for possibleDrop key by key. Lack of key means 0 probability
    [Export]
    public Godot.Collections.Array<float> randomDropProbability = new Godot.Collections.Array<float> { 1.0f };

    [Export]
    public float BaseOpeningCost = 50;

    [Export]
    public float RandomCostPenalty = 10;

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
        if (!isRandomDrop)
        {
            HashSet<ItemsEnum> dropSet = new HashSet<ItemsEnum>(drop);
            dropSet.ExceptWith(possibleDrop);
            if (dropSet.Count > 0)
            {
                GD.PrintErr("ItemOpenable {} contains imposible drop ids {}", this.Name, dropSet);
            }
        }
        else if (randomDropProbability.Sum() > 1.0f)
        {
            GD.PrintErr("ItemOpenable {} has invalid randomDropProbability. It should not exceed 1.0", this.Name);
        }
        else if (randomDropProbability.Count() > possibleDrop.Count())
        {
            GD.PrintErr("ItemOpenable {} randomDropProbability can not be longer then possibleDrop list", this.Name);
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
        foreach (var itemKey in GetDrop())
        {
            if (itemKey == ItemsEnum.None)
            {
                continue;
            }
            PackedScene itemScene;
            if (aIGlobals.availableItems.TryGetValue(itemKey, out itemScene))
            {
                Node2D itemNode = (Node2D)itemScene.Instance();
                itemNode.Position = new Vector2(dropMargin.x * dropI, dropMargin.y);
                this.AddChild(itemNode);
            }
            dropI++;
        }
        OnOpened();
        EmitSignal(nameof(Opened));
        isOpened = true;
        await Task.Delay(800);
        return true;
    }

    public float GetOpeningCost(ItemsEnum itemType)
    {
        if (isRandomDrop)
        {
            if (randomDropProbability.Count() > possibleDrop.IndexOf(itemType))
            {
                float dropProbability = randomDropProbability[possibleDrop.IndexOf(itemType)];
                if (dropProbability == 0.0)
                {
                    GD.PrintErr("Trying to look for item with 0 probability");
                    return BaseOpeningCost + 99999.9f;
                }
                return BaseOpeningCost + (RandomCostPenalty / dropProbability);
            }
        }
        return BaseOpeningCost;
    }

    protected virtual void OnOpened()
    {

    }

    protected List<ItemsEnum> GetDrop()
    {
        if (isRandomDrop)
        {
            Random random = new Random();
            var rand = random.NextDouble();
            var dropI = 0;
            foreach (var prob in randomDropProbability)
            {
                if (rand < prob)
                {
                    return new List<ItemsEnum> { possibleDrop[dropI] };
                }
                dropI++;
            }
            return new List<ItemsEnum>();
        }

        return new List<ItemsEnum>(drop);
    }

}
