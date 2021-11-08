using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GhostHandler : MonoBehaviour
{
    private string selectedStructure = "";
    public Material defaultMaterial;

    public string SelectedStructure { get => selectedStructure; set => selectedStructure = value; }

    public static bool Placing { get => Instance.selectedStructure != ""; }
    private static GhostHandler Instance { get; set; } //Singleton structure.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && selectedStructure != "" && !UIUtility.MouseCoveringStructureSelect)
        {
            GameObject ghostGameObject = new GameObject(selectedStructure + " Ghost");
            Cell CellClickedOn = GridUtility.GetCellAtMousePosition();

            Entity entityInfo = EntityDefinitions.Instance.EntitiesByTypes["StructureEntity"][selectedStructure];
            GhostEntity ghostEntity = ghostGameObject.AddComponent<GhostEntity>();
            entityInfo.CopyProperties(ghostGameObject.GetComponent<GhostEntity>());

            if (!CellClickedOn.SetGhost(ghostEntity)) { Destroy(ghostGameObject); return; }

            Texture2D texture = ResourceHandler.LoadTexture(entityInfo.TexturePath);

            if (ghostEntity.Tileable)
            {
                Cell[] surroundingCells = GridUtility.FindSquareOfCells(1, CellClickedOn.GridLocation);
                foreach (var cell in surroundingCells)
                {
                    if (cell == CellClickedOn) { continue; }
                    if (cell.ContainsGhost && cell.GhostContained.Tileable)
                    {
                        cell.RecheckTileableTexture(CellClickedOn);
                    }
                }
                bool rightFull = surroundingCells.Where(
                    cell => cell.GridLocation.x == CellClickedOn.GridLocation.x + 1 && cell.GridLocation.y == CellClickedOn.GridLocation.y)
                    .ToArray()[0].ContainsGhost;

                bool leftFull = surroundingCells.Where(
                    cell => cell.GridLocation.x == CellClickedOn.GridLocation.x - 1 && cell.GridLocation.y == CellClickedOn.GridLocation.y)
                    .ToArray()[0].ContainsGhost;

                bool topFull = surroundingCells.Where(
                    cell => cell.GridLocation.x == CellClickedOn.GridLocation.x && cell.GridLocation.y == CellClickedOn.GridLocation.y + 1)
                    .ToArray()[0].ContainsGhost;

                bool bottomFull = surroundingCells.Where(
                    cell => cell.GridLocation.x == CellClickedOn.GridLocation.x && cell.GridLocation.y == CellClickedOn.GridLocation.y - 1)
                    .ToArray()[0].ContainsGhost;

                texture = TileableStructureTextureGenerator.GenerateTexture(
                    texture, new Vector2Int(60, 100), new Vector2Int(19, 128 - 75), new Vector2Int(108, 128 - 11), new Vector2Int(texture.width, texture.height),
                    rightFull, leftFull, topFull, bottomFull);
            }

            ghostGameObject.AddComponent<SpriteRenderer>().sprite = GridUtility.CreateSprite(
                texture,
                entityInfo.PivotPoint, entityInfo.SpriteScale);
            ghostGameObject.GetComponent<SpriteRenderer>().material = defaultMaterial;
            ghostGameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
            ghostGameObject.transform.position = GridUtility.GetMouseGridPositionVector();
        }
    }

    public static void SetSelectedStructure(string structureName)
    {
        Instance.selectedStructure = structureName;
        SelectionHandler.DisableSelection();
    }
    public static void ResetSelectedStructure()
    {
        Instance.selectedStructure = "";
        SelectionHandler.EnableSelection();
    }
}
