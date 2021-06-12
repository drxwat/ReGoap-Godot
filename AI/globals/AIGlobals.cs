using Godot;
using System.Collections.Generic;



public class AIGlobals : Node
{
    public Dictionary<ItemsEnum, PackedScene> availableItems = new Dictionary<ItemsEnum, PackedScene>() {
        {ItemsEnum.Key, GD.Load<PackedScene>("res://goap-demo/key/Key.tscn")},
        {ItemsEnum.Chest, GD.Load<PackedScene>("res://goap-demo/chest/Chest.tscn")}
    };
    

    public HashSet<ItemsEnum> openableItems = new HashSet<ItemsEnum>();

}
