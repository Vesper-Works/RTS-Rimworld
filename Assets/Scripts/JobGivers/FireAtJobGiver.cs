using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAtJobGiver : BaseJobGiver
{
    public FireAtJobGiver(Cell commandCell, PlayerEntity entity)
    {
        this.commandCell = commandCell;
        this.entity = entity;
    }

    public override bool Available => entity.Drafted && !commandCell.IsEmpty 
        && commandCell.EntityContained.Selectable && entity.HasRangedWeapon
        && (Vector2.Distance(entity.transform.position, commandCell.WorldPosition) <= entity.Weapon.RangedAttack.Range)
        && commandCell.EntityContained.BlockProjectiles;

    public override void Execute()
    {
        entity.AttackThing(commandCell.EntityContained);
    }
}
