using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlantJobGiver : BaseJobGiver
{
    public CutPlantJobGiver(Cell commandCell, PlayerEntity entity)
    {
        this.commandCell = commandCell;
        this.entity = entity;
    }
    public override bool Available => commandCell.ContainsStructure 
                                      && commandCell.StructureContained.HasComponent(typeof(PlantComponent));

    public override void Execute()
    {
        MoveInfrontOfJob moveInfrontOfJob = new MoveInfrontOfJob(entity, commandCell.GridLocation);
        CutPlantJob cutPlantJob = new CutPlantJob(entity, commandCell);

        entity.AddJob(moveInfrontOfJob);
        entity.AddJob(cutPlantJob);
    }
}
