using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EquipmentViewSetup : MonoBehaviour
{
    public Image EquipmentImage;
    public TextMeshProUGUI EquipmentName;

    public Image LeftAttachementImage;
    public Image RightAttachementImage;
    public Image TopAttachementImage;
    public Image BottomAttachementImage;
  
    public TextMeshProUGUI LeftAttachementText;
    public TextMeshProUGUI RightAttachementText;
    public TextMeshProUGUI TopAttachementText;
    public TextMeshProUGUI BottomAttachementText;
    public void Setup(ItemEntity entity)
    {
        EquipmentImage.sprite = ResourceHandler.LoadSprite(entity.TexturePath);
        EquipmentName.text = entity.Name;
    }
}
