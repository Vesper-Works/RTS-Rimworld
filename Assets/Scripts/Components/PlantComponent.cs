using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantComponent : BaseComponent
{
    private float growthRate;
   [SerializeField] private float growthPercentage = 0;
    private int minStartGrowthPercentage;
    private int maxStartGrowthPercentage;
    private float cutTime;
    private List<Resource> harvestResources;
    private Dictionary<string, int> harvestResourcesReference;
    private int cuttingProgress;
    public float GrowthRate { get => growthRate; set => growthRate = Mathf.Clamp(value, 0, 100); }
    public bool ReadyToHarvest { get => growthPercentage >= 70; }
    public Dictionary<string, int> HarvestResources
    {
        get
        {
            return harvestResourcesReference;
        }
        set
        {
            if (value == null) { return; }
            harvestResources = new List<Resource>();
            harvestResourcesReference = value;
            foreach (var item in harvestResourcesReference)
            {
                ResourceType resourceType = (ResourceType)EntityDefinitions.Instance.ResourceDefinitions[item.Key];
                Resource resource = new Resource();
                resourceType.CopyProperties(resource);
                resource.Amount = item.Value;
                harvestResources.Add(resource);
            }
        }
    }

    public List<Resource> HarvestResourceList { get => harvestResources; }

    public float GrowthPercentage { get => growthPercentage; set => growthPercentage = value; }
    public int MinStartGrowthPercentage { get => minStartGrowthPercentage; set => minStartGrowthPercentage = value; }
    public int MaxStartGrowthPercentage { get => maxStartGrowthPercentage; set => maxStartGrowthPercentage = value; }
    public float CutTime { get => cutTime; set => cutTime = value; }

    public void Harvest()
    {
        //GridUtility.DropItemsAroundPosition(HarvestItemsList, transform.position);
        Destroy(gameObject);
    }
    private void Start()
    {
        if (gameObject.name == "EntitiesDefinitions") { gameObject.SetActive(false); enabled = false; return; }
    }
    public override void Startup()
    {
        if (RandomNumber.CoinFlip())
        {
            gameObject.AddComponent<SpriteRenderer>().flipX = true;
        }
        growthPercentage = (int)(minStartGrowthPercentage + (RandomNumber.Get() / 100f) * (maxStartGrowthPercentage - minStartGrowthPercentage));
        transform.localScale = Vector2.one * GetComponent<Entity>().SpriteScale * (growthPercentage / 100);
        StartCoroutine("GrowingCoroutine");
    }
    public void StartCutting()
    {
        StartCoroutine(CuttingCoroutine());
    }
    private IEnumerator GrowingCoroutine()
    {
        yield return new WaitForSeconds(RandomNumber.Get() / 100f);
        while (growthPercentage < 100)
        {
            growthPercentage += growthRate * Time.deltaTime;
            transform.localScale = new Vector2(transform.localScale.x + (0.01f / growthRate * Time.deltaTime), transform.localScale.y + (0.01f / growthRate * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
    }
    private bool ProgressCutting()
    {
        cuttingProgress++;
        if (cuttingProgress >= cutTime)
        {
            return true;
        }
        return false;
    }
    private IEnumerator CuttingCoroutine()
    {
        while (!ProgressCutting())
        {
            yield return new WaitForSeconds(1);
        }
        if(harvestResources != null && ReadyToHarvest) { ResourceTracker.AddResource(harvestResources); }
    
        Destroy(gameObject);
    }

    public override string ToBasicString()
    {
        return "Growth Percentage: " + Mathf.FloorToInt(GrowthPercentage).ToString() + "\n" + "Ready to harvest: " + ReadyToHarvest;
    }
    public override string ToDetailedString()
    {
        string harvestItems = "";
        foreach (var item in HarvestResourceList)
        {
            harvestItems += "\n";
            harvestItems += item.Name + ": " + item.Amount;
        }
        return "Growth Percentage: " + Mathf.FloorToInt(GrowthPercentage).ToString()
            + "\n" + "Growth Rate: " + growthRate
            + harvestItems;

    }
}

