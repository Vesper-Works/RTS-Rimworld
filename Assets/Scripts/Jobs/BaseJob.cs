using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseJob
{
    protected MoveableEntity entity;
    protected Cell commandCell;

    public bool overrideOrder;
    public abstract bool Execute();
    public virtual void Cancel(BaseJob nextJob)
    {
        //entity.MoveAcrossPath(entity.GridLocation);
    }

    public abstract bool IsFinished { get; }
}
