using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverComponent : BaseComponent
{
    private float effectiveness;

    public float Effectiveness { get => effectiveness; set => effectiveness = value; }

    public override void Startup()
    {
        //Debug.LogWarning(new System.NotImplementedException());
    }

    public override string ToDetailedString()
    {
        return "Cover Effectiveness: " + effectiveness;
    }
    public override string ToBasicString()
    {
        return "Cover Effectiveness: " + effectiveness;
    }
}
