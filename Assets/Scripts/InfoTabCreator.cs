using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTabCreator : MonoBehaviour
{
    public GameObject infoTab;
    public GameObject detailedInfoTab;
    private static InfoTabCreator instance { get; set; } //Singleton structure.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void CreateInfoTab(Entity entity)
    {
        Instantiate(instance.infoTab, entity.transform.position, Quaternion.identity, entity.transform).GetComponent<InfoTab>().DisplayInfo(entity);
    }
    public static void CreateDetailedInfoTab(Entity entity)
    {
        Instantiate(instance.detailedInfoTab, entity.transform.position, Quaternion.identity, instance.transform).GetComponent<DetailedInfoTab>().DisplayInfo(entity);
    }
}
