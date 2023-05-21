using Godot;
using System;
using System.Collections.Generic;
using ReGoap.Core;

public class GoapAction<T, W> : IReGoapAction<T, W>
{
    public string Name = "GoapAction";

    protected bool enabled = false;
    protected ReGoapState<T, W> preconditions;
    protected ReGoapState<T, W> effects;
    public float Cost = 1;

    protected Action<IReGoapAction<T, W>> doneCallback;
    protected Action<IReGoapAction<T, W>> failCallback;
    protected IReGoapAction<T, W> previousAction;
    protected IReGoapAction<T, W> nextAction;

    protected IReGoapAgent<T, W> agent;
    protected bool interruptWhenPossible;

    protected ReGoapState<T, W> settings = null;

    public GoapAction(String name)
    {
        Name = name;
        effects = ReGoapState<T, W>.Instantiate();
        preconditions = ReGoapState<T, W>.Instantiate();

        settings = ReGoapState<T, W>.Instantiate();
    }

	public virtual bool IsActive() => enabled;

	public virtual void PostPlanCalculations(IReGoapAgent<T, W> goapAgent) => 
        agent = goapAgent;

	public virtual bool IsInterruptable() => true;

	public virtual void AskForInterruption() => interruptWhenPossible = true;

	public virtual void Precalculations(GoapActionStackData<T, W> stackData) => 
        agent = stackData.agent;

	public virtual List<ReGoapState<T, W>> GetSettings(GoapActionStackData<T, W> stackData) => 
        new List<ReGoapState<T, W>> { settings };

	public virtual ReGoapState<T, W> GetPreconditions(GoapActionStackData<T, W> stackData) => 
        preconditions;

	public virtual ReGoapState<T, W> GetEffects(GoapActionStackData<T, W> stackData) => 
        effects;

	public virtual float GetCost(GoapActionStackData<T, W> stackData) => Cost;

	public virtual bool CheckProceduralCondition(GoapActionStackData<T, W> stackData) => 
        true;

	public virtual void Run(IReGoapAction<T, W> previous, IReGoapAction<T, W> next, ReGoapState<T, W> settings,
        ReGoapState<T, W> goalState, Action<IReGoapAction<T, W>> done, Action<IReGoapAction<T, W>> fail)
    {
        interruptWhenPossible = false;
        enabled = true;
        doneCallback = done;
        failCallback = fail;
        this.settings = settings;

        previousAction = previous;
        nextAction = next;
    }

    public virtual void PlanEnter(IReGoapAction<T, W> previousAction, IReGoapAction<T, W> nextAction, ReGoapState<T, W> settings, ReGoapState<T, W> goalState)
    {
    }

    public virtual void PlanExit(IReGoapAction<T, W> previousAction, IReGoapAction<T, W> nextAction, ReGoapState<T, W> settings, ReGoapState<T, W> goalState)
    {
    }

	public virtual void Exit(IReGoapAction<T, W> next) => enabled = false;

	public virtual string GetName() => Name;

	public override string ToString() => string.Format("GoapAction('{0}')", Name);

	public virtual string ToString(GoapActionStackData<T, W> stackData)
    {
        string result = string.Format("GoapAction('{0}')", Name);
        if (stackData.settings != null && stackData.settings.Count > 0)
        {
            result += " - ";
            foreach (var pair in stackData.settings.GetValues())
            {
                result += string.Format("{0}='{1}' ; ", pair.Key, pair.Value);
            }
        }
        return result;
    }
}
