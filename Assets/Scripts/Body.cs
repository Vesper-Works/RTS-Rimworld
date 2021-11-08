using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Body
{
    private string name;
    public List<BodyPart> mainBodyParts = new List<BodyPart>();

    public string Name { get => name; set => name = value; }
}