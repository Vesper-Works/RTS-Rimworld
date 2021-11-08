using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailedInfoTab : MonoBehaviour
{
    private TextMeshProUGUI text;
    public void DisplayInfo(Entity entity)
    {
        SelectionHandler.DisableSelection();
        //SelectionHandler.ClearCurrentSelection();

        RectTransform rectTransform = (RectTransform)transform;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(50, 50);
        rectTransform.offsetMax = new Vector2(-50, -50);

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = entity.ToDetailedString();

    }

    public void DestroyThis()
    {
        SelectionHandler.EnableSelection();
        Destroy(gameObject);
    }
}
