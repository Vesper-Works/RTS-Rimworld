using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
public class MoveableEntity : Entity
{
    private float speed;
    private string displayName;
    private string bodyName;
    protected Body body;
    private Cell currentCell;
    protected LineRenderer pathRenderer;
    protected TMPro.TextMeshPro textBox;

    protected ItemEntity currentWeapon;
    protected List<ItemEntity> rangedWeapons = new List<ItemEntity>();
    protected List<ItemEntity> meleeWeapons = new List<ItemEntity>();
    protected Dictionary<string, ItemEntity> clothes = new Dictionary<string, ItemEntity>();
    public int2 GridLocation { get { return GridUtility.WorldToGrid(transform.position); } }

    public float Speed { get => speed; set => speed = value; }
    public float CurrentSpeed { get => speed * GridUtility.GetGridCell(GridLocation).WalkSpeedModifier; }
    public bool Arrived { get => arrived; set => arrived = value; }
    public string DisplayName { get => Name; set => displayName = value; }
    public string Body { get => bodyName; set { body = EntityDefinitions.Instance.BodyDefinitions[value]; bodyName = value; } }
    public Body BodyInstance { get => body; }

    public ItemEntity Weapon { get => currentWeapon; }
    public bool HasWeapon { get => currentWeapon != null; }
    public bool HasRangedWeapon { get => HasWeapon && currentWeapon.GetComponent<RangedAttack>() != null; }
    public bool HasMeleeWeapon { get => HasWeapon && currentWeapon.GetComponent<MeleeAttack>() != null; }
    private float penetrationArmour { get; }
    private float bluntArmour { get; }

    protected Coroutine currentSmallCoroutine;
    protected Coroutine currentBigCoroutine;
    [SerializeField] private bool arrived = true;

    protected override void Start()
    {
        base.Start();
        if (gameObject.name == "EntitiesDefinitions") { Destroy(this); return; }

        #region Initialise path renderer
        pathRenderer = gameObject.AddComponent<LineRenderer>();
        pathRenderer.widthMultiplier = 0.1f;
        pathRenderer.material = GetComponent<SpriteRenderer>().material;
        pathRenderer.startColor = new Color(1, 1, 1, 0.4f);
        pathRenderer.endColor = new Color(1, 1, 1, 0.4f);
        #endregion

        GameObject textGameObject = new GameObject(DisplayName);

        textBox = textGameObject.AddComponent<TMPro.TextMeshPro>();
        textBox.text = DisplayName;
        textBox.fontSize = 3;
        textBox.alignment = TMPro.TextAlignmentOptions.Top;

        RectTransform rectTransform = (RectTransform)textGameObject.transform;
        rectTransform.sizeDelta = new Vector3(2, -0.8f, 1);
        textGameObject.transform.SetParent(transform);
        rectTransform.localPosition = Vector3.zero;

        BlockProjectiles = true;// Possibly change!

        currentCell = GridUtility.GetGridCell(transform.position);
    }

    protected virtual void Update()
    {
        Cell tempCell = GridUtility.GetGridCell(transform.position);
        if (currentCell != tempCell) { ChangedCell(tempCell); }
        currentCell = tempCell;
    }

    private void ChangedCell(Cell newCell)
    {
        if (newCell == null) { Destroy(gameObject); }

        if (!newCell.ContainsMoveableEntity)
        {
            newCell.reservedMoveableEntity = this;
        }
        if (currentCell.reservedMoveableEntity == this)
        {
            currentCell.reservedMoveableEntity = null;
        }
    }

    private BodyPart GetBodyPartHit(BodyPart bodyPart)
    {
        float bodyCoverageHit = RandomNumber.Get() / 100f;
        float chanceTally = 0;
        foreach (var part in bodyPart.attachedBodyParts.OrderByDescending(part => part.Coverage))
        {
            chanceTally += part.Coverage;
            if (chanceTally >= bodyCoverageHit)
            {
                if (part.attachedBodyParts.Count == 0) { return part; }
                else { return GetBodyPartHit(part); }
            }
        }
        return bodyPart;
    }
    private BodyPart GetBodyPartHit(Body bodyPart)
    {
        float bodyCoverageHit = RandomNumber.Get() / 100f;
        float chanceTally = 0;
        foreach (var part in bodyPart.mainBodyParts.OrderByDescending(part => part.Coverage))
        {
            chanceTally += part.Coverage;
            if (chanceTally >= bodyCoverageHit)
            {
                if (part.attachedBodyParts.Count == 0) { return part; }
                else { return GetBodyPartHit(part); }
            }
        }
        return bodyPart.mainBodyParts[0];
    }
    public override void TakeDamage(int damage, float penetration, float speed, float mass)
    {

        BodyPart bodyPartHit = GetBodyPartHit(body);
        speed *= 50;
        Debug.Log("Hit " + Name + " in the " + bodyPartHit.Name);

        if (penetration >= penetrationArmour)
        {
            float leftOverPen = penetration - penetrationArmour;

            float speedAfterPenetration = (leftOverPen / penetration) * speed;

            int actualDamage = Mathf.RoundToInt(damage * (speed / speedAfterPenetration));

            if (bodyPartHit.TakeDamage(actualDamage))
            {
                Debug.Log("Destroyed " + bodyPartHit.Name + "!");
            }
            float bluntEnergy = (0.5f * mass * Mathf.Pow(speedAfterPenetration, 2)) - bluntArmour;
        }
        else
        {
            float bluntEnergy = (0.5f * mass * Mathf.Pow(speed, 2)) - bluntArmour;
        }
    }

    public bool HasSpecificWeapon(ItemEntity weaponReference)
    {
        foreach (var weapon in rangedWeapons)
        {
            if (weapon == weaponReference) { return true; }
        }
        foreach (var weapon in meleeWeapons)
        {
            if (weapon == weaponReference) { return true; }
        }
        return false;
    }
    public void AddRangedWeapon(ItemEntity item)
    {
        item.GetComponent<SpriteRenderer>().sortingOrder = 1;
        rangedWeapons.Add(item);
        ChangeWeapon(item);
    }
    public void AddMeleeWeapon(ItemEntity item)
    {
        item.GetComponent<SpriteRenderer>().sortingOrder = 1;
        meleeWeapons.Add(item);
        ChangeWeapon(item);
    }
    private void ChangeWeapon(ItemEntity item)
    {
        if (rangedWeapons.Contains(item) || meleeWeapons.Contains(item))
        {
            if (currentWeapon != null) { currentWeapon.GetComponent<SpriteRenderer>().enabled = false; }
            currentWeapon = item;
            currentWeapon.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    public bool AttackThing(Entity target)
    {
        RangedAttack rangedAttack; currentWeapon.TryGetComponent(out rangedAttack);
        MeleeAttack meleeAttack; currentWeapon.TryGetComponent(out meleeAttack);

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (rangedAttack != null && distanceToTarget > rangedAttack.Range) { return false; }
        if (rangedAttack == null && distanceToTarget > meleeAttack.Range) { return false; }
        StartCoroutine(AttackingEntity(target, rangedAttack, meleeAttack));
        return true;
    }

    private IEnumerator AttackingEntity(Entity target, RangedAttack rangedAttack, MeleeAttack meleeAttack)
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        Vector2 directionToTarget = (target.transform.position - transform.position).normalized;

        float currentAimTime = 0;
        float aimTime = 1;
        float currentMeleeCooldown = 0;
        float meleeCooldown = 1;

        if (rangedAttack != null)
        {
            while (distanceToTarget <= rangedAttack.Range)// && distanceToTarget > meleeAttack.Range)
            {
                if(target == null) { currentWeapon.ResetRotation(); rangedAttack.CancelShooting(); yield break; }
                currentWeapon.transform.right = target.transform.position - currentWeapon.transform.position;

                if (currentWeapon.transform.right.x < 0 && currentWeapon.transform.right.x != -1) { currentWeapon.GetComponent<SpriteRenderer>().flipY = true; }
                else { currentWeapon.GetComponent<SpriteRenderer>().flipY = false; }

                if (currentAimTime >= aimTime)
                {
                    rangedAttack.Fire(directionToTarget, ID);
                    currentAimTime = 0;
                }
                else 
                {
                    if (!rangedAttack.IsFiring()) { currentAimTime += 0.1f; }
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        if(meleeAttack == null) { currentWeapon.ResetRotation(); yield break; }
        while (distanceToTarget <= meleeAttack.Range)
        {
            if (target == null) { break; }
            currentWeapon.transform.right = target.transform.position - currentWeapon.transform.position;
            if (currentWeapon.transform.right.x < 0 && currentWeapon.transform.right.x != -1) { currentWeapon.GetComponent<SpriteRenderer>().flipY = true; }
            if (currentMeleeCooldown >= meleeCooldown)
            {
                rangedAttack.Fire(directionToTarget, ID);
                currentMeleeCooldown = 0;
            }
            else
            {
                currentMeleeCooldown += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            meleeAttack.Hit(target);
        }
        currentWeapon.ResetRotation();
    }


    public void MoveAcrossPath(int2 end)
    {
        StopCurrentCoroutine();

        if (arrived == false)
        {
            MoveAcrossPath(new List<Cell> { currentCell });
        }

        Arrived = false;

        GridUtility.GetGridCell(GridLocation).reservedMoveableEntity = null;
        GridUtility.GetGridCell(end).reservedMoveableEntity = this;
        StartCoroutine(KeepTryingToGetPath(GridUtility.GeneratePath(GridLocation, end), 0));
    }

    public void MoveInfrontOf(int2 end)
    {
        StopCurrentCoroutine();
        Arrived = false;
        StartCoroutine(KeepTryingToGetPath(GridUtility.GeneratePath(GridLocation, end), 1));
    }

    public void MoveAcrossPath(List<Cell> path)
    {
        pathRenderer.positionCount = path.Count + 1;
        Vector3[] pathPositions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            pathPositions[i] = path[i].WorldPosition;
        }
        pathRenderer.SetPositions(pathPositions);
        StopCurrentCoroutine();
        if (path.Count == 0)
        {
            return;
        }
        GridUtility.GetGridCell(GridLocation).reservedMoveableEntity = null;
        path[path.Count - 1].reservedMoveableEntity = this;

        Arrived = false;
        currentBigCoroutine = StartCoroutine(MoveAcrossPathCoroutine(path.ToArray()));
    }

    public void MoveToLocation(Vector2 end)
    {
        Arrived = false;
        StopCurrentCoroutine();
        currentSmallCoroutine = StartCoroutine(MoveToLocationCoroutine(end));

    }
    public void MoveToLocation(float x, float y)
    {
        Arrived = false;
        StopCurrentCoroutine();
        currentSmallCoroutine = StartCoroutine(MoveToLocationCoroutine(x, y));

    }
    public void StopCurrentCoroutine()
    {
        if (currentSmallCoroutine == null) { return; }
        StopCoroutine(currentSmallCoroutine);
        currentSmallCoroutine = null;

    }
    private IEnumerator MoveAcrossPathCoroutine(Cell[] nodeList)
    {
        Cell currentNode;

        for (int i = 0; i < nodeList.Length; i++)
        {
            currentNode = nodeList[i];
            SendNextLocationToTravelTo(currentNode.GridLocation.x, currentNode.GridLocation.y);
            while (!Mathf.Approximately(transform.position.x, currentNode.WorldPosition.x) || !Mathf.Approximately(transform.position.y, currentNode.WorldPosition.y))
            {
                pathRenderer.SetPosition(pathRenderer.positionCount - 1, transform.position);
                yield return new WaitForFixedUpdate();
            }
        }

        MoveableEntity entityAtLocation = nodeList[nodeList.Length - 1].reservedMoveableEntity;
        if (entityAtLocation != null)
        {
            //Leave cell
        }
        Arrived = true;
        pathRenderer.positionCount = 0;
        Debug.Log("Arrived");
    }

    private IEnumerator MoveToLocationCoroutine(Vector2 end)
    {
        while (!Mathf.Approximately(transform.position.x, end.x) || !Mathf.Approximately(transform.position.y, end.y))
        {
            transform.position = Vector2.MoveTowards(transform.position, end, CurrentSpeed);
            yield return new WaitForFixedUpdate();
        }
        currentSmallCoroutine = null;
    }

    private IEnumerator MoveToLocationCoroutine(float x, float y)
    {
        while (!Mathf.Approximately(transform.position.x, x) || !Mathf.Approximately(transform.position.y, y))
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(x, y), CurrentSpeed);
            yield return new WaitForFixedUpdate();
        }
        currentSmallCoroutine = null;
    }

    private IEnumerator KeepTryingToGetPath(float key, int spacesBefore)
    {
        List<Cell> path = PathfindingBuffer.GetPath(key);
        while (path == null)
        {
            yield return new WaitForEndOfFrame();
            path = PathfindingBuffer.GetPath(key);
        }
        path.RemoveRange(path.Count - spacesBefore, spacesBefore);
        MoveAcrossPath(path);
    }


    private void SendNextLocationToTravelTo(float x, float y)
    {
        var data = new SFSObject();
        data.PutInt("ID", ID);
        data.PutFloat("x", x);
        data.PutFloat("y", y);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("SendNodeMoveRequest", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }

    private void SendNextLocationToTravelTo(int2 location)
    {
        var data = new SFSObject();
        data.PutInt("ID", ID);
        data.PutFloat("x", location.x);
        data.PutFloat("y", location.y);
        SmartFoxConnection.Connection.Send(new ExtensionRequest("SendNodeMoveRequest", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }
    public override void OnSelect()
    {
        base.OnSelect();
        pathRenderer.enabled = true;
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        pathRenderer.enabled = false;
    }
}
