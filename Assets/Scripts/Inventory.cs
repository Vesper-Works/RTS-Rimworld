using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory
{
    List<ItemEntity> items = new List<ItemEntity>();
    List<Resource> resources = new List<Resource>();

    public void AddItem(ItemEntity itemToAdd)
    {
        if (!items.Exists(x => x.Name == itemToAdd.Name))
        {
            items.Add(itemToAdd);
        }
        else
        {
            items.Where(x => x.Name == itemToAdd.Name).ToList()[0].Amount += itemToAdd.Amount;
        }
        LogItems();
    }

    public bool HasItemsRequired(List<ItemEntity> itemsNeeded)
    {
        foreach (var item in itemsNeeded)
        {
            if(!items.Exists(x => x.Name == item.Name && x.Amount >= item.Amount))
            {
                return false;
            }
        }
        return true;
    }

    public bool RemoveItems(List<ItemEntity> itemsToRemove)
    {
        foreach (var item in itemsToRemove)
        {
            if (!items.Exists(x => x.Name == item.Name && x.Amount >= item.Amount))
            {
                return false;
            }

        }
       
        foreach (var item in itemsToRemove)
        {
            if (items.Exists(x => x.Name == item.Name && x.Amount > item.Amount))
            {
                items.Where(x => x.Name == item.Name && x.Amount > item.Amount).ToList()[0].Amount -= item.Amount;
            }
            else
            {
                items.Remove(items.Where(x => x.Name == item.Name && x.Amount == item.Amount).ToList()[0]);
            }
        }
   
        LogItems();
        return true;
    }

    public bool ItemHasBeenAdded(ItemEntity itemToCheck)
    {
        if (items.Exists(x => x.Name == itemToCheck.Name)) { return items.Where(x => x.Name == itemToCheck.Name).ToList()[0].Amount >= itemToCheck.Amount; }
        return false;
    }

    public void AddResource(Resource ResourceToAdd)
    {
        if (!resources.Exists(x => x.Name == ResourceToAdd.Name))
        {
            resources.Add(ResourceToAdd);
        }
        else
        {
            resources.Where(x => x.Name == ResourceToAdd.Name).ToList()[0].Amount += ResourceToAdd.Amount;
        }
    }

    public bool HasResourcesRequired(List<Resource> ResourcesNeeded)
    {
        foreach (var Resource in ResourcesNeeded)
        {
            if (!resources.Exists(x => x.Name == Resource.Name && x.Amount >= Resource.Amount))
            {
                return false;
            }
        }
        return true;
    }

    public bool RemoveResources(List<Resource> ResourcesToRemove)
    {
        foreach (var Resource in ResourcesToRemove)
        {
            if (!resources.Exists(x => x.Name == Resource.Name && x.Amount >= Resource.Amount))
            {
                return false;
            }

        }

        foreach (var Resource in ResourcesToRemove)
        {
            if (resources.Exists(x => x.Name == Resource.Name && x.Amount > Resource.Amount))
            {
                resources.Where(x => x.Name == Resource.Name && x.Amount > Resource.Amount).ToList()[0].Amount -= Resource.Amount;
            }
            else
            {
                resources.Remove(resources.Where(x => x.Name == Resource.Name && x.Amount == Resource.Amount).ToList()[0]);
            }
        }

        return true;
    }

    public bool ResourceHasBeenAdded(Resource ResourceToCheck)
    {
        if (resources.Exists(x => x.Name == ResourceToCheck.Name)) { return resources.Where(x => x.Name == ResourceToCheck.Name).ToList()[0].Amount >= ResourceToCheck.Amount; }
        return false;
    }

    private void LogItems()
    {
        foreach (var item in items)
        {
            Debug.Log(item.Name + ": " + item.Amount);
        }
    }
}
