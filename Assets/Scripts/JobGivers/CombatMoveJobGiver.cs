using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMoveJobGiver : BaseJobGiver
{
    public CombatMoveJobGiver(Cell commandCell, PlayerEntity entity)
    {
        this.commandCell = commandCell;
        this.entity = entity;
    }
    public override bool Available => entity.Drafted && !commandCell.Impassable;

    public override void Execute()
    {
        entity.AddJob(new MoveJob(entity, commandCell.GridLocation));
    }
}
