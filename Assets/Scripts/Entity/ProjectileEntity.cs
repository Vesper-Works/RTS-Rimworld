using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEntity : Entity
{
    private int damage;
    private float penetration;
    private float mass;
    private Vector3 velocity = Vector3.zero;
    private float speed;
    private new Rigidbody2D rigidbody;
    private int shooterID;
    private bool aimingAtStructure;
    private Cell currentCell;
    public int Damage { get => damage; set => damage = value; }
    public float Penetration { get => penetration; set => penetration = value; }
    public float Mass { get => mass; set => mass = value; }

    protected override void Start()
    {
        if (gameObject.name == "EntitiesDefinitions") { enabled = false; return; }
        rigidbody = GetComponent<Rigidbody2D>();
    }
    public void SetInMotion(Vector2 velocity, float speed, int shooterID)
    {
        this.velocity = velocity * Time.fixedDeltaTime;
        this.speed = speed;
        this.shooterID = shooterID;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void FixedUpdate()
    {
        if (gameObject.name == "EntitiesDefinitions") { enabled = false; return; }
        rigidbody.MovePosition(transform.position + velocity);
    }

    private void Update()
    {
        if (gameObject.name == "EntitiesDefinitions") { enabled = false; return; }
        Cell tempCell = GridUtility.GetGridCell(transform.position);
        if (currentCell != tempCell) { ChangedCell(tempCell); }
        currentCell = tempCell;
    }

    private void ChangedCell(Cell newCell)
    {
        //other.TryGetComponent<CoverComponent>(out CoverComponent cover);
        if (newCell == null) { Destroy(gameObject); return; }
        Entity entity = newCell.EntityContained;

        if (entity == null || !entity.BlockProjectiles) { return; }

        StrikeTarget(entity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer.Equals(5)
            || other.gameObject.name.Equals("MouseCollider")
            || other == GameController.GetEntityFromID(shooterID).GetComponent<BoxCollider2D>()
            || other.gameObject.layer.Equals(8)) { return; }

        //Do cover stuff here
        Entity entity = other.GetComponent<Entity>();
        StrikeTarget(entity);
    }


    private void StrikeTarget(Entity target)
    {
        target.TakeDamage(damage, penetration, speed, mass);

        Destroy(gameObject);
    }

}
