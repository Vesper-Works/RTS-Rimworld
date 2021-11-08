using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildJob : BaseJob
{
    private PlayerEntity playerEntity;
    public override bool IsFinished => commandCell.ContainsStructure;
    public BuildJob(PlayerEntity playerEntity, Cell commandCell)
    {
        this.playerEntity = playerEntity;
        this.commandCell = commandCell;
    }

    public override void Cancel(BaseJob nextJob)
    {
        if (commandCell.ContainsGhost)
        { commandCell.GhostContained.StopAllCoroutines(); }
        base.Cancel(nextJob);
    }

    public override bool Execute()
    {
        if (!commandCell.GhostContained.HasNeededResources)
        {
            List<Resource> resourcesNeeded = commandCell.GhostContained.ResourcesNeeded;
            if (!ResourceTracker.HasEnoughResource(resourcesNeeded.ToArray()))
            {
                GameMessageCreator.CreateFloatingText("Not enough resources", entity.transform.position);
                return false;
            }

            ResourceTracker.SubtractResource(commandCell.GhostContained.ResourcesNeeded.ToArray());
        }
        commandCell.GhostContained.StartBuilding();

        return true;
    }



}