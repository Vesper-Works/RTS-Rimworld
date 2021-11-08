using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MoveInfrontOfJob : BaseJob
{
    private int2 endLocation;
    private int2 trueEndLocation;
    private MoveInfrontOfJob extentionMoveOrder;
    public MoveInfrontOfJob(MoveableEntity _entity, int2 _endLocation)
    {
        endLocation = _endLocation;
        entity = _entity;
    }
    public override bool Execute()
    {
        if (!GridUtility.IsValidLocation(endLocation)) { return false; }

        trueEndLocation = GridUtility.FindClosestAdjacentPassableCell(endLocation, entity.GridLocation).GridLocation;

        int count = 0;
        while (GridUtility.GetGridCell(trueEndLocation).ContainsMoveableEntity)
        {
            count++;
            if (count >= 20) { break; }
            foreach (var cell in GridUtility.FindSquareOfCells(count, endLocation))
            {
                if (!cell.ContainsMoveableEntity) { trueEndLocation = cell.GridLocation; break; }
            }                     
        }
        
        entity.MoveAcrossPath(trueEndLocation);     
        return true;
    }

    public override void Cancel(BaseJob nextJob)
    {
        if (extentionMoveOrder != null)
        {
            extentionMoveOrder.Cancel(nextJob);
        }
        else
        {
            GridUtility.GetGridCell(trueEndLocation).reservedMoveableEntity = null;
            if (nextJob.GetType() != typeof(MoveJob) && nextJob.GetType() != typeof(MoveInfrontOfJob))
            {
                GridUtility.GetGridCell(entity.GridLocation).reservedMoveableEntity = entity;
            }
        }
    }

    public override bool IsFinished => entity.Arrived;
}
