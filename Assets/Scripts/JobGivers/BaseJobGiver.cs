using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseJobGiver
{
    protected Cell commandCell;
    protected PlayerEntity entity;
    public abstract bool Available { get; }

    public abstract void Execute();
}
