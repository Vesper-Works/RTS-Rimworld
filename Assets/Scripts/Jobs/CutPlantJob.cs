using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlantJob : BaseJob
{
    public CutPlantJob(PlayerEntity entity, Cell commandCell)
    {
        this.entity = entity;
        this.commandCell = commandCell;
    }

    public override bool IsFinished => !commandCell.ContainsStructure;

    public override bool Execute()
    {
        if (commandCell.StructureContained.TryGetComponent(typeof(PlantComponent), out BaseComponent plantComponent))
        {
            ((PlantComponent)plantComponent).StartCutting();
            return true;
        }
        return false;
    }
}
