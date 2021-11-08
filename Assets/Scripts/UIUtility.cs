using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtility
{
    public static bool MouseCoveringTabButtons { get => Input.mousePosition.y / Camera.main.pixelHeight < 0.05f; }


    public static bool MouseCoveringStructureSelect { get => Input.mousePosition.y / Camera.main.pixelHeight < 0.25f; }

}
