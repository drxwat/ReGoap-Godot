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

    public bool isIdling()
    {
        return currentActionState == null && !IsPlanning;
    }

    public bool CalculateGoal()
    {
        return CalculateNewGoal();
    }

    public bool ForceCalculateNewGoal()
    {
        return CalculateNewGoal(true);
    }

}
