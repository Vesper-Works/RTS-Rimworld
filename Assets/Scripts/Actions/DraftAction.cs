using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftAction : BaseAction
{
    public override void Execute(Entity entity)
    {
        PlayerEntity playerEntity = (PlayerEntity)entity;
        if (!playerEntity.Drafted)
        {
            playerEntity.Draft();
        }
        else
        {
            playerEntity.UnDraft();
        }
    }
}
