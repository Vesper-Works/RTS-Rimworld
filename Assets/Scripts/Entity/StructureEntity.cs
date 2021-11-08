using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
public class StructureEntity : Entity
{
    private bool impassable;
    private bool tileable;
    private float walkSpeedModifier = 1;
    private string buildTab = "";
    private int commonness = 0;
    private List<Resource> resourcesNeeded;
    private int buildTime;

    public float WalkSpeedModifier { get => walkSpeedModifier; set => walkSpeedModifier = value; }
    public bool Impassable { get => impassable; set => impassable = value; }
    public string BuildTab { get => buildTab; set => buildTab = value; }
    public int Commonness { get => commonness; set => commonness = value; }
    public List<Resource> ResourcesNeeded { get => resourcesNeeded; set => resourcesNeeded = value; }
    public int BuildTime { get => buildTime; set => buildTime = value; }
    public bool Tileable { get => tileable; set => tileable = value; }
    public Texture2D Texture { get => GetComponent<SpriteRenderer>().sprite.texture; }

    public void ReTextureForTiles()
    {
        if (!tileable) { return; }
        Texture2D texture = ResourceHandler.LoadTexture(TexturePath);
        Cell[] surroundingCells = GridUtility.FindSquareOfCells(1, transform.position.ToInt2());
        Cell spawnCell = GridUtility.GetGridCell(transform.position);
        foreach (var cell in surroundingCells)
        {
            if (cell == spawnCell) { continue; }
            if (cell.ContainsGhost && cell.GhostContained.Tileable)
            {
                cell.RecheckTileableTexture(spawnCell);
            }
        }
        Cell cellRight = surroundingCells.
            Where(cell => cell.GridLocation.x == spawnCell.GridLocation.x + 1 &&
            cell.GridLocation.y == spawnCell.GridLocation.y).ToArray()[0];

        bool rightFull = (cellRight.ContainsGhost && cellRight.GhostContained.Tileable) ||
            (cellRight.ContainsStructure && cellRight.StructureContained.Tileable);

        Cell cellLeft = surroundingCells.
            Where(cell => cell.GridLocation.x == spawnCell.GridLocation.x - 1 &&
            cell.GridLocation.y == spawnCell.GridLocation.y).ToArray()[0];

        bool leftFull = (cellLeft.ContainsGhost && cellLeft.GhostContained.Tileable) ||
            (cellLeft.ContainsStructure && cellLeft.StructureContained.Tileable);

        Cell cellAbove = surroundingCells.
            Where(cell => cell.GridLocation.x == spawnCell.GridLocation.x &&
            cell.GridLocation.y == spawnCell.GridLocation.y + 1).ToArray()[0];

        bool topFull = (cellAbove.ContainsGhost && cellAbove.GhostContained.Tileable) ||
            (cellAbove.ContainsStructure && cellAbove.StructureContained.Tileable);

        Cell cellBelow = surroundingCells.
        Where(cell => cell.GridLocation.x == spawnCell.GridLocation.x &&
        cell.GridLocation.y == spawnCell.GridLocation.y - 1).ToArray()[0];

        bool bottomFull = (cellBelow.ContainsGhost && cellBelow.GhostContained.Tileable) ||
            (cellBelow.ContainsStructure && cellBelow.StructureContained.Tileable);

        texture = TileableStructureTextureGenerator.GenerateTexture(
            texture, new Vector2Int(60, 100), new Vector2Int(19, 128 - 75), new Vector2Int(108, 128 - 11), new Vector2Int(texture.width, texture.height),
            rightFull, leftFull, topFull, bottomFull);

        if(TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.sprite =
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(PivotPoints[0], PivotPoints[1]), texture.width / SpriteScale);
        }
        else
        {
            gameObject.AddComponent<SpriteRenderer>().sprite =
               Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
               new Vector2(PivotPoints[0], PivotPoints[1]), texture.width / SpriteScale);
        }
        
    }
    //public override string ToBasicString()
    //{
    //    string extraInfoText = "";

    //    return Name + "\n" + extraInfoText;
    //}
    //public override string ToDetailedString()
    //{
    //    string extraInfoText = "";

    //    return base.ToDetailedString() + Name + "\n" + description + "\n" + extraInfoText;
    //}
}
