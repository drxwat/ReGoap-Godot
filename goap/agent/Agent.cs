using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;

public class Agent : GoapAgent<string, object>
{
	public Agent(
		string name,
		IReGoapMemory<string, object> _memory,
		List<IReGoapAction<string, object>> _actions,
		List<IReGoapGoal<string, object>> _goals) : base(name, _memory, _actions, _goals)
	{

	}

}
