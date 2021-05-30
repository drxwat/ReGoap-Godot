using ReGoap.Planner;

public class PlanManager : ReGoapPlannerManager<string, object> {

    public PlanManager() {
        PlannerSettings = new ReGoapPlannerSettings();
        PlannerSettings.UsingDynamicActions = true;
    }
    
}