using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeconstructableComponent : BaseComponent
{
    public override void Startup()
    {
        Debug.LogWarning(new System.NotImplementedException());
    }

    public override string ToDetailedString()
    {
        throw new System.NotImplementedException(); //Return how long it takes and what you get back
    }

}
