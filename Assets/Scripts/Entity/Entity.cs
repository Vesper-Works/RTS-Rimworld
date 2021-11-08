using Sfs2X.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private new string name;
    private string label;
    private string description;
    private User owner;
    private int iD;
    public GameObject selectedGraphic;
    private bool flammable = true;
    private string texPath;
    protected int maxHealth;
    protected int currentHealth;
    private bool selected;
    private Color colour;
    private bool blockProjectiles;
    private bool varyLocation;
    private bool selectable = true;

    private string pivotPoint = "0.5, 0.5";
    private float spriteScale = 1;
    private List<BaseComponent> components = new List<BaseComponent>();
    private List<BaseAction> actions = new List<BaseAction>();


    public Color Colour
    {
        get
        {
            if (colour == Color.clear) { return Color.white; }
            else { return colour; }
        }
        set => colour = value;
    }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public string TexturePath
    {
        get
        {
            if (texPath == null) { return "MissingTexture"; }
            return texPath;
        }
        set => texPath = value;
    }
    public string Label { get => label; set => label = value; }
    public int ID { get => iD; set => iD = value; }
    public string Name { get => name; set => name = value; }
    public User Owner { get => owner; set => owner = value; }
    public bool Selected { get => selected; set => selected = value; }
    public string Description { get => description; set => description = value; }
    public bool Flammable { get => flammable; set => flammable = value; }
    public List<BaseComponent> Components { get => components; set => components = value; }
    public string PivotPoint { get => pivotPoint; set => pivotPoint = value; }
    public float[] PivotPoints
    {
        get
        {
            string[] stringPivotPoints = PivotPoint.Split(',');
            return new float[] { Mathf.Clamp01(float.Parse(stringPivotPoints[0])), Mathf.Clamp01(float.Parse(stringPivotPoints[1])) };
        }
    }
    public float SpriteScale { get => spriteScale; set => spriteScale = value; }
    public List<BaseAction> Actions { get => actions; set => actions = value; }
    public bool BlockProjectiles { get => blockProjectiles; set => blockProjectiles = value; }
    public bool VaryLocation { get => varyLocation; set => varyLocation = value; }
    public bool Selectable { get => selectable; set => selectable = value; }

    protected virtual void Start()
    {
        if (gameObject.name == "EntitiesDefinitions") { Destroy(this); return; }
        components.Clear();
        foreach (var item in GetComponents<Component>())
        {
            if (item.GetType().IsSubclassOf(typeof(BaseComponent)))
            {
                components.Add((BaseComponent)item);
            }
        }
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(int damage, float penetration, float speed, float mass)
    {
        currentHealth -= damage;
        Debug.Log(Name + " took " + damage + " damage!");
        if (currentHealth <= 0)
        {
            GridUtility.GetGridCell(transform.position).DestroyEntity();
        }

    }

    public bool TryGetComponent(Type componentType, out BaseComponent component)
    {
        component = components.Find(x => x.GetType() == componentType);
        if (component == null)
        {
            return false;
        }
        return true;
    }
    public bool HasComponent(Type componentType)
    {
        return components.Exists(x => x.GetType() == componentType);
    }
    public virtual string ToBasicString()
    {
        string extraInfoText = "Health: " + currentHealth + "/" + maxHealth + "\n";
        foreach (var item in components)
        {
            if (item.ToBasicString() == "") { continue; }
            extraInfoText += item.ToBasicString();
            extraInfoText += "\n";
        }
        if (Owner != null)
        {
            return Name + "\n" + "Owner: " + Owner.Name + "\n" + extraInfoText;
        }
        return Name + "\n" + extraInfoText;
    }
    public string ToDetailedString()
    {
        string extraInfoText = "Health: " + currentHealth + "/" + maxHealth + "\n";
        foreach (var item in components)
        {
            if (item.ToDetailedString() == "") { continue; }
            extraInfoText += item.ToDetailedString();
            extraInfoText += "\n\n";
        }
        if (Owner != null)
        {
            return Name + "\n" + "Owner: " + Owner.Name + "\n" + description + "\n" + extraInfoText;
        }
        return Name + "\n" + description + "\n" + extraInfoText;
    }

    public virtual void OnDeselect()
    {
        Selected = false;
        selectedGraphic.SetActive(false);
        Debug.Log("Deselected: " + "\n" + ToString());
        foreach (var action in actions)
        {
            ActionButtonHandler.RemoveAction(action, this);
        }
    }
    public virtual void OnSelect()
    {
        Selected = true;
        selectedGraphic.SetActive(true);
        InfoTabCreator.CreateInfoTab(this);
        Debug.Log("Selected: " + "\n" + ToString());
        foreach (var action in actions)
        {
            ActionButtonHandler.AddAction(action, this);
        }
    }

}
