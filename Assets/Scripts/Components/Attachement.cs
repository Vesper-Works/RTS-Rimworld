using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachement : BaseComponent
{
    private float sightInaccuracy;
    private float stability;
    private float barrelSpread;

    public float SightInaccuracy { get => sightInaccuracy; set => sightInaccuracy = value; }
    public float Stability { get => stability; set => stability = value; }
    public float BarrelSpread { get => barrelSpread; set => barrelSpread = value; }
    public override void Startup()
    {
        throw new System.NotImplementedException();
    }

    public override string ToDetailedString()
    {
        throw new System.NotImplementedException();
    }
}
