using System;
using System.Collections.Generic;
using ReGoap.Core;
using ReGoap.Planner;
using ReGoap.Utilities;
using Godot;


// behaviour that should be added once (and only once) to a gameobject in your unity's scene
public class ReGoapPlannerManager<T, W> : Node
{
    public static ReGoapPlannerManager<T, W> Instance;

    public bool MultiThread;
    public int ThreadsCount = 1;
    private ReGoapPlannerThread<T, W>[] planners;

    private List<ReGoapPlanWork<T, W>> doneWorks;
    private System.Threading.Thread [] threads;

    public ReGoapPlannerSettings PlannerSettings;

    public ReGoapLogger.DebugLevel LogLevel = ReGoapLogger.DebugLevel.Full;

    public int NodeWarmupCount = 1000;
    public int StatesWarmupCount = 10000;

    public override void _Ready()
    {
        ReGoapNode<T, W>.Warmup(NodeWarmupCount);
        ReGoapState<T, W>.Warmup(StatesWarmupCount);

        ReGoapLogger.Level = LogLevel;
        if (Instance != null)
        {
            var errorString =
                "[GoapPlannerManager] Trying to instantiate a new manager but there can be only one per scene.";
            ReGoapLogger.LogError(errorString);
            return;
        }
        Instance = this;

        doneWorks = new List<ReGoapPlanWork<T, W>>();
        ReGoapPlannerThread<T, W>.WorksQueue = new Queue<ReGoapPlanWork<T, W>>();
        planners = new ReGoapPlannerThread<T, W>[ThreadsCount];
        threads = new System.Threading.Thread [ThreadsCount];

        if (MultiThread)
        {
            ReGoapLogger.Log(String.Format("[GoapPlannerManager] Running in multi-thread mode ({0} threads).", ThreadsCount));
            for (int i = 0; i < ThreadsCount; i++)
            {
                planners[i] = new ReGoapPlannerThread<T, W>(PlannerSettings, OnDonePlan);
                var thread = new System.Threading.Thread (planners[i].MainLoop);
                thread.Start();
                threads[i] = thread;
            }
        } // no threads run
        else
        {
            ReGoapLogger.Log("[GoapPlannerManager] Running in single-thread mode.");
            planners[0] = new ReGoapPlannerThread<T, W>(PlannerSettings, OnDonePlan);
        }
    }

    public override void _ExitTree() 
    {
        foreach (var planner in planners)
        {
            planner?.Stop();
        }
        // should wait here?
        foreach (var thread in threads)
        {
            thread?.Abort();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        ReGoapLogger.Level = LogLevel;
        if (doneWorks.Count > 0)
        {
            lock (doneWorks)
            {
                foreach (var work in doneWorks)
                {
                    work.Callback(work.NewGoal);
                }
                doneWorks.Clear();
            }
        }
        if (!MultiThread)
        {
            planners[0].CheckWorkers();
        }
    }

    // called in another thread
    private void OnDonePlan(ReGoapPlannerThread<T, W> plannerThread, ReGoapPlanWork<T, W> work, IReGoapGoal<T, W> newGoal)
    {
        work.NewGoal = newGoal;
        lock (doneWorks)
        {
            doneWorks.Add(work);
        }
        if (work.NewGoal != null && ReGoapLogger.Level == ReGoapLogger.DebugLevel.Full)
        {
            ReGoapLogger.Log("[GoapPlannerManager] Done calculating plan, actions list:");
            var i = 0;
            foreach (var action in work.NewGoal.GetPlan())
            {
                ReGoapLogger.Log(string.Format("{0}: {1}", i++, action.Action));
            }
        }
    }

    public ReGoapPlanWork<T, W> Plan(IReGoapAgent<T, W> agent, IReGoapGoal<T, W> blacklistGoal, Queue<ReGoapActionState<T, W>> currentPlan, Action<IReGoapGoal<T, W>> callback)
    {
        var work = new ReGoapPlanWork<T, W>(agent, blacklistGoal, currentPlan, callback);
        lock (ReGoapPlannerThread<T, W>.WorksQueue)
        {
            ReGoapPlannerThread<T, W>.WorksQueue.Enqueue(work);
        }
        return work;
    }
}
