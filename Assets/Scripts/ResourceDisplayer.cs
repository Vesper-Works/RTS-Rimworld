using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResourceDisplayer : MonoBehaviour
{
    private Image resourceIcon;
    private string resourceName;
    private TextMeshProUGUI textMesh;
    private int resourceAmount;
    private void Awake()
    {
        resourceIcon = GetComponent<Image>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetIcon(string iconPath)
    {
        //if(resourceIcon == null) { resourceIcon = GetComponent<Image>(); }
        resourceIcon.sprite = ResourceHandler.LoadSprite(iconPath);
    }
    public void SetName(string name)
    {
        resourceName = name;
    }
    public void SetAmount(int amount)
    {
        resourceAmount = amount;
        textMesh.text = resourceName + ": " + resourceAmount.ToString();    
    }
    public void AddAmount(int amount)
    {
        resourceAmount += amount;
        textMesh.text = resourceName + ": " + resourceAmount.ToString();
    }
    public void SubtractAmount(int amount)
    {
        resourceAmount -= amount;
        textMesh.text = resourceName + ": " + resourceAmount.ToString();
    }
    public string GetName()
    {
        return resourceName;
    }
    public int GetAmount()
    {
        return resourceAmount;
    }
}
