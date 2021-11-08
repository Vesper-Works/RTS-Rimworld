using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction
{
    private string texPath;
    private string text;
    private char hotkey;
    public string TexturePath
    {
        get
        {
            if (texPath == null || texPath == "") { return "MissingTexture"; }
            return texPath;
        }
        set => texPath = value;
    }

    public string Text { get => text; set => text = value; }
    public char Hotkey { get => hotkey; set => hotkey = value; }

    public void CreateButton(Entity entity)
    {
        ActionButtonHandler.AddAction(this, entity);
    }
    public abstract void Execute(Entity entity);
}
