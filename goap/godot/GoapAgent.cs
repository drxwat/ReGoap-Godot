using Godot;
using System;
using ReGoap.Core;
using System.Collections.Generic;

public class GoapAgent<T, W> : IReGoapAgent<T, W>
{
    protected bool isActive = true;
    protected List<IReGoapGoal<T, W>> goals;
    protected List<IReGoapAction<T, W>> actions;
    protected IReGoapMemory<T, W> memory;
    protected IReGoapGoal<T, W> currentGoal; // TODO: set on planning done see OnDonePlanning, CalculateNewGoal
    protected List<ReGoapActionState<T, W>> startingPlan;
    protected Dictionary<T, W> planValues;

    public GoapAgent(
        IReGoapMemory<T, W> _memory,
        List<IReGoapAction<T, W>> _actions,
        List<IReGoapGoal<T, W>> _goals)
    {
        memory = _memory;
        actions = _actions;
        goals = _goals;
    }

    public virtual bool IsActive()
    {
        return isActive;
    }

    public virtual IReGoapGoal<T, W> GetCurrentGoal()
    {
        return currentGoal;
    }

    public virtual void WarnPossibleGoal(IReGoapGoal<T, W> goal)
    {
        // if ((currentGoal != null) && (goal.GetPriority() <= currentGoal.GetPriority()))
        //     return;
        // if (currentActionState != null && !currentActionState.Action.IsInterruptable())
        // {
        //     interruptOnNextTransition = true;
        //     currentActionState.Action.AskForInterruption();
        // }
        // else
        //     CalculateNewGoal();
    }

    public virtual List<ReGoapActionState<T, W>> GetStartingPlan()
    {
        return startingPlan;
    }


    public virtual W GetPlanValue(T key)
    {
        return planValues[key];
    }

    public virtual bool HasPlanValue(T key)
    {
        return planValues.ContainsKey(key);
    }

    public virtual void SetPlanValue(T key, W value)
    {
        planValues[key] = value;
    }

    public virtual List<IReGoapGoal<T, W>> GetGoalsSet()
    {
        return goals;
    }

    public virtual List<IReGoapAction<T, W>> GetActionsSet()
    {
        return actions;
    }

    public virtual IReGoapMemory<T, W> GetMemory()
    {
        return memory;
    }

    public virtual ReGoapState<T, W> InstantiateNewState()
    {
        return ReGoapState<T, W>.Instantiate();
    }
}
