using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildJobGiver : BaseJobGiver
{
    public BuildJobGiver(Cell commandCell, PlayerEntity entity)
    {
        this.commandCell = commandCell;
        this.entity = entity;
    }

    public override bool Available
    {
        get
        {
            if (!commandCell.ContainsGhost || entity.Drafted)
            {
                return false;
            }

            List<Resource> resourcesNeeded = commandCell.GhostContained.ResourcesNeeded;
            if (ResourceTracker.HasEnoughResource(resourcesNeeded.ToArray()))
            {
                return true;
            }
            GameMessageCreator.CreateFloatingText("Not enough resources", new Vector2(entity.transform.position.x + 1, entity.transform.position.y));
            return false;

            //if (!entity.Inventory.HasItemsRequired(itemsNeeded))
            //{
            //    Cell[] cellsToCheck = GridUtility.FindSquareOfCells(20, entity.GridLocation);
            //    foreach (var cell in cellsToCheck)
            //    {
            //        if (cell != null && cell.ContainsItem && itemsNeeded.Exists(item => item.Name == cell.ItemContained.Name))
            //        {
            //            int index = itemsNeeded.FindIndex(item => item.Name == cell.ItemContained.Name);
            //            if (itemsGot[index]) { continue; }
            //            itemsAmountProgress[index] += cell.ItemContained.Amount;
            //            if (itemsAmountProgress[index] >= itemsNeeded[index].Amount)
            //            {
            //                itemsGot[index] = true;
            //            }
            //        }
            //    }
            //    foreach (var gotItemCheck in itemsGot)
            //    {
            //        if (!gotItemCheck)
            //        {
            //            return false;
            //        }
            //    }
            //    return true;
            //}
            //return true;
        }
    }

    public override void Execute()
    {
        MoveInfrontOfJob moveInfrontOfJob = new MoveInfrontOfJob(entity, commandCell.GridLocation);
        BuildJob buildJob = new BuildJob(entity, commandCell);

        //List<ItemEntity> itemsNeeded = commandCell.GhostContained.ItemsNeeded;
        //bool[] itemsGot = new bool[itemsNeeded.Count];
        //int[] itemsAmountProgress = new int[itemsNeeded.Count];

        //if (!entity.Inventory.HasItemsRequired(itemsNeeded))
        //{
        //    List<BaseJob> moveAndPickupJobs = new List<BaseJob>();
        //    Cell[] cellsToCheck = GridUtility.FindSquareOfCells(20, entity.GridLocation);
        //    foreach (var cell in cellsToCheck)
        //    {
        //        if (cell != null && cell.ContainsItem && itemsNeeded.Exists(item => item.Name == cell.ItemContained.Name))
        //        {
        //            int index = itemsNeeded.FindIndex(item => item.Name == cell.ItemContained.Name);
        //            if (itemsGot[index]) { continue; }
        //            itemsAmountProgress[index] += cell.ItemContained.Amount;
        //            moveAndPickupJobs.Add(new MoveInfrontOfJob(entity, cell.GridLocation));
        //            moveAndPickupJobs.Add(new PickupItemStackJob(entity, cell));
        //            if (itemsAmountProgress[index] >= itemsNeeded[index].Amount)
        //            {
        //                itemsGot[index] = true;
        //            }
        //        }
        //    }
        //    foreach (var gotItemCheck in itemsGot)
        //    {
        //        if (!gotItemCheck)
        //        {
        //            return;
        //        }
        //    }
        //    entity.AddJob(moveAndPickupJobs);
        //}

        entity.AddJob(moveInfrontOfJob);
        entity.AddJob(buildJob);
    }
}