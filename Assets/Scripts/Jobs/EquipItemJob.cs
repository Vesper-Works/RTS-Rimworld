using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItemJob : BaseJob
{
   
    public override bool IsFinished => entity.HasSpecificWeapon(commandCell.ItemContained);

    public EquipItemJob(MoveableEntity moveableEntity, Cell commandCell)
    {
        this.entity = moveableEntity;
        this.commandCell = commandCell;
    }

    public override void Cancel(BaseJob nextJob)
    {
        base.Cancel(nextJob);
    }

    public override bool Execute()
    {
        if (!commandCell.ContainsItem) { return false; }
        commandCell.ItemContained.Equip(entity);
        commandCell.RemoveEntity();
        return true;
    }
}
