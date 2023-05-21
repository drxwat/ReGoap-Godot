using Godot;
using System.Linq;
using ReGoap.Core;
using System.Collections.Generic;
using ReGoap.Utilities;

public class GoapAgent<T, W> : IReGoapAgent<T, W>
{
	public string Name;
	public ulong CalculationDelay = 500;
	public bool BlackListGoalOnFailure = false;
	protected bool isActive = true;
	protected List<IReGoapGoal<T, W>> goals;
	protected List<IReGoapAction<T, W>> actions;
	protected IReGoapMemory<T, W> memory;
	protected IReGoapGoal<T, W> currentGoal;
	protected List<ReGoapActionState<T, W>> startingPlan;
	protected Dictionary<T, W> planValues;
	protected ReGoapActionState<T, W> currentActionState;

	protected Dictionary<IReGoapGoal<T, W>, uint> goalBlacklist = new Dictionary<IReGoapGoal<T, W>, uint>();
	protected List<IReGoapGoal<T, W>> possibleGoals;
	protected bool possibleGoalsDirty;

	protected bool startedPlanning;
	protected ReGoapPlanWork<T, W> currentReGoapPlanWorker;
	public bool IsPlanning
	{
		get { return startedPlanning && currentReGoapPlanWorker.NewGoal == null; }
	}

	protected bool interruptOnNextTransition;

	protected ulong lastCalculationTime = 0;

	public GoapAgent(
		string name,
		IReGoapMemory<T, W> _memory,
		List<IReGoapAction<T, W>> _actions,
		List<IReGoapGoal<T, W>> _goals)
	{
		Name = name;
		memory = _memory;
		actions = _actions;
		goals = _goals;
		CalculateNewGoal(true);
	}

	public virtual bool IsActive() => isActive;

	public virtual IReGoapGoal<T, W> GetCurrentGoal() => currentGoal;


	public virtual List<ReGoapActionState<T, W>> GetStartingPlan() => startingPlan;


	public virtual W GetPlanValue(T key) => planValues[key];

	public virtual bool HasPlanValue(T key) => planValues.ContainsKey(key);

	public virtual void SetPlanValue(T key, W value) => planValues[key] = value;

	public virtual List<IReGoapGoal<T, W>> GetGoalsSet()
	{
		if (possibleGoalsDirty)
			UpdatePossibleGoals();
		return possibleGoals;
	}

	public virtual List<IReGoapAction<T, W>> GetActionsSet() => actions;

	public virtual IReGoapMemory<T, W> GetMemory() => memory;

	public virtual ReGoapState<T, W> InstantiateNewState() => 
		ReGoapState<T, W>.Instantiate();

	// Sets All available goals for the Agent
	public virtual void SetGoalsSet(List<IReGoapGoal<T, W>> _goals)
	{
		goals = _goals;
		possibleGoalsDirty = true;
	}

	public virtual void Disable()
	{
		if (currentActionState != null)
		{
			currentActionState.Action.Exit(null);
			currentActionState = null;
			currentGoal = null;
		}
	}

	protected virtual void UpdatePossibleGoals()
	{
		possibleGoalsDirty = false;
		if (goalBlacklist.Count > 0)
		{
			possibleGoals = new List<IReGoapGoal<T, W>>(goals.Count);
			foreach (var goal in goals)
				if (!goalBlacklist.ContainsKey(goal))
				{
					possibleGoals.Add(goal);
				}
				else if (goalBlacklist[goal] < OS.GetTicksMsec())
				{
					goalBlacklist.Remove(goal);
					possibleGoals.Add(goal);
				}
		}
		else
		{
			possibleGoals = goals;
		}
	}

	protected virtual void TryWarnActionFailure(IReGoapAction<T, W> action)
	{
		if (action.IsInterruptable())
			WarnActionFailure(action);
		else
			action.AskForInterruption();
	}

	protected virtual bool CalculateNewGoal(bool forceStart = false)
	{
		if (IsPlanning)
			return false;
		if (!forceStart && (OS.GetTicksMsec() - lastCalculationTime <= CalculationDelay))
			return false;
		lastCalculationTime = OS.GetTicksMsec();
		interruptOnNextTransition = false;
		UpdatePossibleGoals();
		//var watch = System.Diagnostics.Stopwatch.StartNew();
		startedPlanning = true;
		currentReGoapPlanWorker = ReGoapPlannerManager<T, W>.Instance.Plan(this, BlackListGoalOnFailure ? currentGoal : null,
			currentGoal?.GetPlan(), OnDonePlanning);

		return true;
	}

	protected virtual void OnDonePlanning(IReGoapGoal<T, W> newGoal)
	{
		startedPlanning = false;
		currentReGoapPlanWorker = default(ReGoapPlanWork<T, W>);
		if (newGoal == null)
		{
			if (currentGoal == null)
			{
				ReGoapLogger.LogWarning("GoapAgent " + this + " could not find a plan.");
			}
			return;
		}

		currentActionState?.Action.Exit(null);
		currentActionState = null;
		currentGoal = newGoal;
		if (startingPlan != null)
		{
			for (int i = 0; i < startingPlan.Count; i++)
			{
				startingPlan[i].Action.PlanExit(i > 0 ? startingPlan[i - 1].Action : null, i + 1 < startingPlan.Count ? startingPlan[i + 1].Action : null, startingPlan[i].Settings, currentGoal.GetGoalState());
			}
		}
		startingPlan = currentGoal.GetPlan().ToList();
		ClearPlanValues();
		for (int i = 0; i < startingPlan.Count; i++)
		{
			startingPlan[i].Action.PlanEnter(i > 0 ? startingPlan[i - 1].Action : null, i + 1 < startingPlan.Count ? startingPlan[i + 1].Action : null, startingPlan[i].Settings, currentGoal.GetGoalState());
		}
		currentGoal.Run(WarnGoalEnd);
		PushAction();
	}

	public static string PlanToString(IEnumerable<IReGoapAction<T, W>> plan)
	{
		var result = "GoapPlan(";
		var reGoapActions = plan as IReGoapAction<T, W>[] ?? plan.ToArray();
		for (var index = 0; index < reGoapActions.Length; index++)
		{
			var action = reGoapActions[index];
			result += string.Format("'{0}'{1}", action, index + 1 < reGoapActions.Length ? ", " : "");
		}
		result += ")";
		return result;
	}

	public virtual void WarnActionEnd(IReGoapAction<T, W> thisAction)
	{
		GD.Print("Action END " + thisAction);
		if (currentActionState != null && thisAction != currentActionState.Action)
			return;
		PushAction();
	}

	protected virtual void PushAction()
	{
		if (interruptOnNextTransition)
		{
			CalculateNewGoal();
			return;
		}
		var plan = currentGoal.GetPlan();
		if (plan.Count == 0)
		{
			if (currentActionState != null)
			{
				currentActionState.Action.Exit(currentActionState.Action);
				currentActionState = null;
			}
			CalculateNewGoal();
		}
		else
		{
			var previous = currentActionState;
			currentActionState = plan.Dequeue();
			IReGoapAction<T, W> next = null;
			if (plan.Count > 0)
				next = plan.Peek().Action;
			previous?.Action.Exit(currentActionState.Action);
			currentActionState.Action.Run(previous?.Action, next, currentActionState.Settings, currentGoal.GetGoalState(), WarnActionEnd, WarnActionFailure);
		}
	}

	public virtual void WarnActionFailure(IReGoapAction<T, W> thisAction)
	{
		GD.Print("ACTION FAILED " + thisAction);
		if (currentActionState != null && thisAction != currentActionState.Action)
		{
			ReGoapLogger.LogWarning(string.Format("[GoapAgent] Action {0} warned for failure but is not current action.", thisAction));
			return;
		}
		if (BlackListGoalOnFailure)
			goalBlacklist[currentGoal] = (uint)OS.GetTicksMsec() + (uint)currentGoal.GetErrorDelay();
		CalculateNewGoal(true);
	}

	public virtual void WarnGoalEnd(IReGoapGoal<T, W> goal)
	{
		GD.Print("GOAL END " + goal);
		if (goal != currentGoal)
		{
			ReGoapLogger.LogWarning(string.Format("[GoapAgent] Goal {0} warned for end but is not current goal.", goal));
			return;
		}
		CalculateNewGoal();
	}

	public virtual void WarnPossibleGoal(IReGoapGoal<T, W> goal)
	{
		if ((currentGoal != null) && (goal.GetPriority() <= currentGoal.GetPriority()))
			return;
		if (currentActionState != null && !currentActionState.Action.IsInterruptable())
		{
			interruptOnNextTransition = true;
			currentActionState.Action.AskForInterruption();
		}
		else
			CalculateNewGoal();
	}

	protected virtual void ClearPlanValues()
	{
		if (planValues == null)
			planValues = new Dictionary<T, W>();
		else
		{
			planValues.Clear();
		}
	}

	public override string ToString() => string.Format("GoapAgent('{0}')", Name);

}
