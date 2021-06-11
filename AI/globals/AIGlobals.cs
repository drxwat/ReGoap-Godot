using Godot;
using System.Collections.Generic;

public class AIGlobals : Node
{
    public Dictionary<string, PackedScene> availableItems = new Dictionary<string, PackedScene>() {
        {"Key", GD.Load<PackedScene>("res://goap-demo/key/Key.tscn")},
        {"Chest", GD.Load<PackedScene>("res://goap-demo/chest/Chest.tscn")}
    };

    public HashSet<string> openableItems = new HashSet<string>();

}
