using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GhostEntity : Entity
{
    private int currentBuildProgress;
    private bool hasNeededResources;
    private bool tileable;
    private int buildTime;
    private List<Resource> resourcesNeeded;
    public bool HasNeededResources { get => hasNeededResources; set => hasNeededResources = value; }
    public int BuildTime { get => buildTime; set => buildTime = value; }
    public List<Resource> ResourcesNeeded { get => resourcesNeeded; set => resourcesNeeded = value; }
    public bool Tileable { get => tileable; set => tileable = value; }

    public Texture2D Texture { get => GetComponent<SpriteRenderer>().sprite.texture; }

    protected override void Start()
    {
        base.Start();
        BlockProjectiles = false;
    }
    public void StartBuilding()
    {
        StartCoroutine(BuildStructure());
    }
    private bool ProgressBuilding()
    {
        currentBuildProgress++;
        if (currentBuildProgress >= BuildTime)
        {
            return true;
        }
        return false;
    }
    public IEnumerator BuildStructure()
    {
        while (!ProgressBuilding())
        {
            yield return new WaitForSeconds(1);
        }
        GameController.CreateEntity("StructureEntity", Name, transform.position, false);
        Destroy(gameObject);
    }
}