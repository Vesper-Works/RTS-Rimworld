using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Cell
{   //-----------------------------------------------
    // Fields
    //-----------------------------------------------

    private string name;
    public Cell previousCell;
    private bool passable;
    private bool defaultPassable;
    private bool inVision = true;
    private Entity entityContained;
    public float gCost;
    private float walkSpeedModifier = 1;
    private Vector2 worldPosition;
    private int2 gridLocation;
    public float hCost;
    private string texPath;

    public MoveableEntity reservedMoveableEntity;

    //-----------------------------------------------
    // Properties
    //-----------------------------------------------
    public float fCost { get { return hCost + (gCost * (1 / walkSpeedModifier)); } }
    public Texture2D Texture { get => ResourceHandler.LoadTexture(texPath); }
    public string TexturePath { get => texPath; set => texPath = value; }
    public bool Impassable { get => !passable; set => passable = !value; }
    public Vector2 WorldPosition { get => worldPosition; set => worldPosition = value; }
    public int2 GridLocation { get => gridLocation; set => gridLocation = value; }
    public float WalkSpeedModifier { get => walkSpeedModifier; set => walkSpeedModifier = value; }
    public string Name { get => name; set => name = value; }
    public bool ContainsItem { get => entityContained != null && entityContained.GetType() == typeof(ItemEntity); }
    public bool ContainsStructure { get => entityContained != null && entityContained.GetType() == typeof(StructureEntity); }
    public bool ContainsGhost { get => entityContained != null && entityContained.GetType() == typeof(GhostEntity); }
    public bool IsEmpty { get => entityContained == null; }
    public bool ContainsMoveableEntity { get => reservedMoveableEntity != null; }


    public bool DefaultPassable { get => defaultPassable; set => defaultPassable = value; }
    public Entity EntityContained { get => entityContained; }
    public GhostEntity GhostContained { get { if (ContainsGhost) { return (GhostEntity)entityContained; } return null; } }
    public ItemEntity ItemContained { get { if (ContainsItem) { return (ItemEntity)entityContained; } return null; } }
    public StructureEntity StructureContained { get { if (ContainsStructure) { return (StructureEntity)entityContained; } return null; } }
    public bool InVision { get => inVision; set => inVision = value; }

    //-----------------------------------------------
    // Methods
    //-----------------------------------------------
    public Cell(int x, int y)
    {
        gridLocation.x = x;
        gridLocation.y = y;
    }
    public Cell()
    {
    }
    public void RecheckTileableTexture(Cell cellClickedOn)
    {
        bool rightFull = false;
        bool leftFull = false;
        bool bottomFull = false;
        bool topFull = false;
        if (cellClickedOn.GridLocation.x == GridLocation.x + 1 && cellClickedOn.GridLocation.y == GridLocation.y) { rightFull = true; }
        if (cellClickedOn.GridLocation.x == GridLocation.x - 1 && cellClickedOn.GridLocation.y == GridLocation.y) { leftFull = true; }
        if (cellClickedOn.GridLocation.y == GridLocation.y + 1 && cellClickedOn.GridLocation.x == GridLocation.x) { topFull = true; }
        if (cellClickedOn.GridLocation.y == GridLocation.y - 1 && cellClickedOn.GridLocation.x == GridLocation.x) { bottomFull = true; }

        Texture2D oldTexture = GhostContained.Texture;
        Texture2D newTexture = TileableStructureTextureGenerator.GenerateTexture(
                            oldTexture, new Vector2Int(60, 100), new Vector2Int(19, 128 - 75), new Vector2Int(108, 128 - 11), new Vector2Int(oldTexture.width, oldTexture.height),
                            rightFull, leftFull, topFull, bottomFull);

        GhostContained.GetComponent<SpriteRenderer>().sprite = GridUtility.CreateSprite(
                newTexture,
                GhostContained.PivotPoint, GhostContained.SpriteScale);
    }
    public void SendCellToOtherPlayers() //Texture, Passable, Entity
    {
        var data = new SFSObject();
        data.PutUtfString("texturePath", TexturePath);
        data.PutBool("passable", passable);
        if (ContainsItem)
        {
            data.PutInt("itemAmount", ItemContained.Amount);
        }
        SmartFoxConnection.Connection.Send(new ExtensionRequest("UpdateTile", data, SmartFoxConnection.Connection.LastJoinedRoom));
    }

    public bool SetEntity(Entity entity)
    {
        if (IsEmpty && !entity.GetType().IsSubclassOf(typeof(MoveableEntity)))
        {
            entityContained = entity;
            if (entity.GetType() == typeof(StructureEntity))
            {
                Impassable = ((StructureEntity)entity).Impassable;
                WalkSpeedModifier *= ((StructureEntity)entity).WalkSpeedModifier;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveEntity()
    {
        if (!IsEmpty)
        {
            entityContained = null;
            passable = DefaultPassable;
        }
    }
    public void DestroyEntity()
    {
        if (!IsEmpty)
        {      
            passable = DefaultPassable;
            UnityEngine.Object.Destroy(entityContained.gameObject);
   
            if(ContainsStructure && StructureContained.Tileable)
            {
                foreach (var cell in GridUtility.FindSquareOfCells(1, entityContained.transform.position.ToInt2()))
                {
                    if (cell.ContainsStructure && cell.StructureContained.Tileable)
                    {
                        cell.StructureContained.ReTextureForTiles();
                    }
                }
            }
       
            entityContained = null;
        }
    }
    public bool SetGhost(GhostEntity ghost)
    {
        if (IsEmpty && !ContainsGhost)
        {
            entityContained = ghost;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveGhost()
    {
        if (ContainsGhost)
        {
            UnityEngine.Object.Destroy(entityContained.gameObject);
            entityContained = null;
        }
    }

    public string ToBasicString()
    {
        string returnString = name + "\n" + (walkSpeedModifier * 100);
        if (IsEmpty) { return returnString; }
        return returnString + "\n" + entityContained.ToBasicString();
    }
}
