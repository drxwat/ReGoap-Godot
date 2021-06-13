    using Godot;

    public class GoapGoalAdvanced<T, W> : GoapGoal<T, W>
    {
        public ulong WarnDelay = 2;
        private ulong warnCooldown;

        // Should be called in physics process
        public virtual void Update()
        {
            if (planner != null && !planner.IsPlanning() && OS.GetUnixTime() > warnCooldown)
            {
                warnCooldown = OS.GetUnixTime() + WarnDelay;
                var currentGoal = planner.GetCurrentGoal();
                var plannerPlan = currentGoal == null ? null : currentGoal.GetPlan();
                var equalsPlan = ReferenceEquals(plannerPlan, plan);
                var isGoalPossible = IsGoalPossible();
                // check if this goal is not active but CAN be activated
                //  or
                // if this goal is active but isn't anymore possible
                if ((!equalsPlan && isGoalPossible) || (equalsPlan && !isGoalPossible))
                    planner.GetCurrentAgent().WarnPossibleGoal(this);
            }
        }
    }