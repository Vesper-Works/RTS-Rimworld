using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeaponJobGiver : BaseJobGiver
{
    public EquipWeaponJobGiver(Cell commandCell, PlayerEntity entity)
    {
        this.commandCell = commandCell;
        this.entity = entity;
    }

    public override bool Available => commandCell.ContainsItem;

    public override void Execute()
    {
        MoveInfrontOfJob moveInfrontOfJob = new MoveInfrontOfJob(entity, commandCell.GridLocation);
        EquipItemJob equipItemJob = new EquipItemJob(entity, commandCell);

        entity.AddJob(moveInfrontOfJob);
        entity.AddJob(equipItemJob);
    }
}
