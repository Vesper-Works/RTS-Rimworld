using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MouseLocation : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    void Update()
    {
        var cell = GridUtility.GetCellAtMousePosition();
        if (cell != null)
        {
            textMesh.text = cell.ToBasicString();
        }
        transform.position = GridUtility.GetMouseWorldPosition();

    }
}
