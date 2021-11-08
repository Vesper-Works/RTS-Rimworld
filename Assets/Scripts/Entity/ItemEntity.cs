using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ItemEntity : Entity //No amounts
{
    private int amount = 1;
    private float mass;
    private int maxStackAmount;
    private TextMeshPro amountText;
    private bool inGame;
    public int Amount
    {
        get => amount; 
        set
        {
            amount = value;
            if (inGame) { amountText.text = amount.ToString(); }
            
        }
    }
    public float Mass { get => mass; set => mass = value; }
    public int MaxStackAmount { get => maxStackAmount; set => maxStackAmount = value; }

    protected override void Start()
    {
        base.Start();
        if (gameObject.name == "EntitiesDefinitions") { enabled = false; return; }
        inGame = true;
        GameObject textGameObject = new GameObject();
        textGameObject.transform.SetParent(transform);
        textGameObject.transform.position = transform.position;
        amountText = textGameObject.AddComponent<TextMeshPro>();
        amountText.fontSize = 2f;
        ((RectTransform)amountText.transform).sizeDelta = new Vector2(1, 0);
        amountText.text = amount.ToString();
    }

    private string bodyPart;
    private bool weapon;
    public string BodyPart { get => bodyPart; set => bodyPart = value; }
    public bool Weapon { get => weapon; set => weapon = value; }

    public RangedAttack RangedAttack { get => GetComponent<RangedAttack>(); }
    public MeleeAttack MeleeAttack { get => GetComponent<MeleeAttack>(); }

    public BaseAttack MainAttack
    {
        get
        {
            if (GetComponent<RangedAttack>() != null) { return GetComponent<RangedAttack>(); }
            return GetComponent<MeleeAttack>();
        }
    }

    public bool CanEquip(MoveableEntity entity)
    {
        if (GetComponent<RangedAttack>() != null)
        {

        }
        if (GetComponent<MeleeAttack>() != null)
        {

        }

        foreach (var bodyParts in entity.BodyInstance.mainBodyParts)
        {
            if (SearchForBodyPart(bodyPart, bodyParts)) { return true; }
        }

        return false;
    }
    public void Equip(MoveableEntity entity)
    {
        //if (!CanEquip(entity)) { return; }

        if (GetComponent<RangedAttack>() != null)
        {
            entity.AddRangedWeapon(this);
        }
        else if (GetComponent<MeleeAttack>() != null)
        {
            entity.AddMeleeWeapon(this);
        }
        else
        {
            //Armour or whatnot. Look at body layer, find it and apply it.
        }
        transform.SetParent(entity.transform);
        transform.localPosition = new Vector2(0, 0);
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        GetComponent<SpriteRenderer>().flipY = false;
    }

    private bool SearchForBodyPart(string name, BodyPart bodyPart)
    {
        if (bodyPart.Name.Contains(name))
        {
            return true;
        }
        foreach (var item in bodyPart.attachedBodyParts)
        {
            return SearchForBodyPart(name, item);
        }
        return false;
    }
}
