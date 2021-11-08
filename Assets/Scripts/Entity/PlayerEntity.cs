using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlayerEntity : MoveableEntity
{
    protected Queue<BaseJob> jobs;
    protected BaseJob currentJob;
    private readonly Inventory inventory = new Inventory();
    private bool drafted;
    private Type[] jobTypes;
    private List<MoveableEntity> targets = new List<MoveableEntity>();
    public bool Drafted { get => drafted; set => drafted = value; }

    public Inventory Inventory { get => inventory; }

    private void Awake()
    {
        jobs = new Queue<BaseJob>();
        jobTypes = Assembly.GetAssembly(typeof(BaseJobGiver)).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(typeof(BaseJobGiver))).ToArray();
    }

    protected override void Update()
    {
        base.Update();
        ProcessJobs();
        if (currentJob != null && currentJob.IsFinished) { currentJob = null; }
        if (Selected) { ListenForCommands(); }
        //if (currentJob != null && currentJob.IsFinished) { Debug.Log(currentJob.IsFinished); }
    }
    protected void ProcessJobs()
    {
        if (currentJob != null && !currentJob.IsFinished) { return; }

        if (jobs.Count == 0) { return; }/*Process order if: current order is null or if it is finished
                                                                 and there are orders to be processed  */
        currentJob = jobs.Dequeue();
        if (!currentJob.Execute())
        {
            jobs.Clear();
            currentJob = null;
        }
    }

    private void WatchForTargets()
    {
        targets.Clear();

        foreach (var entity in Physics2D.OverlapCircleAll(transform.position, currentWeapon.MainAttack.Range))
        {
            MoveableEntity target = entity.GetComponent<MoveableEntity>();
            if (target == null) { continue; }

            targets.Add(target);
        }
    }

    public void AddJob(BaseJob job)
    {
        jobs.Enqueue(job);
    }

    public void AddJob(List<BaseJob> jobsList)
    {

        foreach (BaseJob job in jobsList)
        {
            jobs.Enqueue(job);
        }
    }

    public void CancelAction(BaseJob nextJob)
    {
        jobs.Clear();
        StopCurrentCoroutine();
        StopAllCoroutines();
        pathRenderer.positionCount = 0;
        if (currentJob != null) { currentJob.Cancel(nextJob); currentJob = null; }
        Arrived = true;
    }

    private void ListenForCommands()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Cell commandCell = GridUtility.GetCellAtMousePosition();

            foreach (var jobGiverType in jobTypes)
            {
                BaseJobGiver jobGiver = (BaseJobGiver)Activator.CreateInstance(jobGiverType, commandCell, this);
                if (jobGiver.Available)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        CancelAction(currentJob);
                    }
                    jobGiver.Execute();
                }
            }          
        }
    }
    public void Draft()
    {
        drafted = true;
        textBox.fontStyle = TMPro.FontStyles.Underline;
        CancelAction(currentJob);
        if(HasWeapon)
        {
            currentWeapon.ResetRotation();
            currentWeapon.gameObject.SetActive(true);
        }       
    }
    public void UnDraft()
    {
        drafted = false;
        textBox.fontStyle = TMPro.FontStyles.Normal;
        CancelAction(currentJob);
        if (HasWeapon)
        {
            currentWeapon.gameObject.SetActive(false);
        }
           
    }
}
