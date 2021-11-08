using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : BaseComponent
{
    private float range;

    public float Range { get => range; set => range = value; }

    public override void Startup()
    {
        throw new System.NotImplementedException();
    }

    public override string ToDetailedString()
    {
        throw new System.NotImplementedException();
    }
}
