using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MoveJob : BaseJob
{
    private int2 endLocation;
    private MoveInfrontOfJob extentionMoveOrder;
    public MoveJob(MoveableEntity _entity, int2 _endLocation)
    {
        endLocation = _endLocation;
        entity = _entity;
    }

    public override bool Execute()
    {
        if (!GridUtility.IsValidLocation(endLocation)) { return false; }
        if (GridUtility.GetGridCell(endLocation).reservedMoveableEntity != null)// && entity.GetType() == typeof(PlayerEntity))
        {
            extentionMoveOrder = new MoveInfrontOfJob(entity, endLocation);
            extentionMoveOrder.Execute();
            //PlayerEntity playerEntity = (PlayerEntity)entity;
            //playerEntity.AddJob(new MoveInfrontOfJob(entity, endLocation));
        }
        else
        {
            entity.MoveAcrossPath(endLocation);
        }
        return true;
    }

    public override void Cancel(BaseJob nextJob)
    {
        if (extentionMoveOrder != null) { extentionMoveOrder.Cancel(nextJob); }
        else
        {
            GridUtility.GetGridCell(endLocation).reservedMoveableEntity = null;
            if (nextJob.GetType() != typeof(MoveJob) && nextJob.GetType() != typeof(MoveInfrontOfJob))
            {
                GridUtility.GetGridCell(entity.GridLocation).reservedMoveableEntity = entity;
            }
        }

    }

    public override bool IsFinished => entity.Arrived;
}
