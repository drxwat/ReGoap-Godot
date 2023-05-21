using System;
using ReGoap.Core;
using ReGoap.Planner;
using System.Collections.Generic;
using System.Linq;

public class GoapGoal<T, W> :  IReGoapGoal<T, W>
{
        public string Name = "GenericGoal";
        public float Priority = 1;
        public float ErrorDelay = 0.5f;

        public bool WarnPossibleGoal = true;

        protected ReGoapState<T, W> goal = ReGoapState<T, W>.Instantiate();
        protected Queue<ReGoapActionState<T, W>> plan;
        protected IGoapPlanner<T, W> planner;

	public virtual string GetName() => Name;

	public virtual float GetPriority() => Priority;

	public virtual bool IsGoalPossible() => WarnPossibleGoal;

	public virtual Queue<ReGoapActionState<T, W>> GetPlan() => plan;

	public virtual ReGoapState<T, W> GetGoalState() => goal;

	public virtual void SetPlan(Queue<ReGoapActionState<T, W>> path) => plan = path;

	public void Run(Action<IReGoapGoal<T, W>> callback)
        {
        }

	public virtual void Precalculations(IGoapPlanner<T, W> goapPlanner) => 
        planner = goapPlanner;

	public virtual float GetErrorDelay() => ErrorDelay;

	public static string PlanToString(IEnumerable<IReGoapAction<T, W>> plan)
        {
            var result = "GoapPlan(";
            var reGoapActions = plan as IReGoapAction<T, W>[] ?? plan.ToArray();
            var end = reGoapActions.Length;
            for (var index = 0; index < end; index++)
            {
                var action = reGoapActions[index];
                result += string.Format("'{0}'{1}", action, index + 1 < end ? ", " : "");
            }
            result += ")";
            return result;
        }

	public override string ToString() => string.Format("GoapGoal('{0}')", Name);
}
