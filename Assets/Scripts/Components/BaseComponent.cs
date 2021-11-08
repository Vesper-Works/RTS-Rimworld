using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseComponent : MonoBehaviour
{
    public BaseComponent DeepCopy()
    {
        BaseComponent other = (BaseComponent)this.MemberwiseClone();
        
        return other;
    }

    private void Awake()
    {
    }
    public abstract void Startup();

    public virtual string ToBasicString()
    {
        return "";
    }
    public abstract string ToDetailedString();
}
