using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    private string iconPath;
    private string name;
    private int amount;
    private string description;

    public string IconPath { get => iconPath; set => iconPath = value; }
    public string TexturePath { get => iconPath; set => iconPath = value; }
    public string Name { get => name; set => name = value; }
    public int Amount { get => amount; set => amount = value; }
    public string Description { get => description; set => description = value; }
}
