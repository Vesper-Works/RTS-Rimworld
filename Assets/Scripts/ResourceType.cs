using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceType
{
    private string iconPath;
    private string name;
    private string description;
    private float mass;

    public string IconPath { get => iconPath; set => iconPath = value; }
    public string TexturePath { get => iconPath; set => iconPath = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public float Mass { get => mass; set => mass = value; }
}
