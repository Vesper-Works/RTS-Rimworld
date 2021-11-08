using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{

    public GameObject resourceDisplay;
    private List<ResourceDisplayer> resources = new List<ResourceDisplayer>();

    private static ResourceTracker Instance { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        foreach (var item in EntityDefinitions.Instance.ResourceDefinitions)
        {
            ResourceDisplayer resourceDisplayer = Instantiate(resourceDisplay, transform).GetComponent<ResourceDisplayer>();
            resourceDisplayer.SetName(item.Value.Name);
            resourceDisplayer.SetIcon(item.Value.IconPath);
            resourceDisplayer.SetAmount(0);
            resources.Add(resourceDisplayer);
        }
    }

    public static void AddResource(string resourceName, int amount)
    {
        for (int i = 0; i < Instance.resources.Count; i++)
        {
            if (Instance.resources[i].GetName() == resourceName)
            {
                Instance.resources[i].AddAmount(amount);
            }
        }
    }
    public static void AddResource(Resource resourceToAdd)
    {
        if (Instance.TryGetResourceOfName(resourceToAdd.Name, out ResourceDisplayer tempResourceDisplayer))
        {
            tempResourceDisplayer.AddAmount(resourceToAdd.Amount);
        }
    }
    public static void AddResource(List<Resource> resourcesToAdd)
    {
        foreach (var resource in resourcesToAdd)
        {
            if (Instance.TryGetResourceOfName(resource.Name, out ResourceDisplayer tempResourceDisplayer))
            {
                tempResourceDisplayer.AddAmount(resource.Amount);
            }
        }
       
    }
    public static void SubtractResource(Resource resourceToSubtract)
    {
        if (Instance.TryGetResourceOfName(resourceToSubtract.Name, out ResourceDisplayer tempResourceDisplayer))
        {
            tempResourceDisplayer.SubtractAmount(resourceToSubtract.Amount);
        }
    }
    public static void SubtractResource(Resource[] resourcesToSubtract)
    {
        for (int i = 0; i < resourcesToSubtract.Length; i++)
        {
            SubtractResource(resourcesToSubtract[i]);
        }
    }
    public static bool HasEnoughResource(Resource resourceNeeded)
    {
        for (int i = 0; i < Instance.resources.Count; i++)
        {
            if (Instance.resources[i].GetName() == resourceNeeded.Name && Instance.resources[i].GetAmount() >= resourceNeeded.Amount)
            {
                return true;
            }
        }
        Debug.Log("Not enough " + resourceNeeded.Name + ": " + resourceNeeded.Amount + " / " + Instance.GetResourceOfName(resourceNeeded.Name).GetAmount());
        return false;
    }
    public static bool HasEnoughResource(Resource[] resourcesNeeded)
    {
        for (int k = 0; k < resourcesNeeded.Length; k++)
        {
            if (!HasEnoughResource(resourcesNeeded[k]))
            {
                return false;
            }
        }
        return true;
    }

    private ResourceDisplayer GetResourceOfName(string resourceName)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].GetName() == resourceName)
            {
                return resources[i];
            }
        }
        return null;
    }

    private bool TryGetResourceOfName(string resourceName, out ResourceDisplayer resource)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].GetName() == resourceName)
            {
                resource = resources[i];
                return true;
            }
        }

        resource = null;
        return false;
    }
}
