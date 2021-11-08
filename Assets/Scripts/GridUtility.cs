using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
public static class GridUtility
{
    static public float GeneratePath(int2 start, int2 end)
    {
        float randomNumber = UnityEngine.Random.Range(float.MinValue, float.MaxValue);
        Grid.Instance.FindPath(start, end, randomNumber);
        return randomNumber;
    }

    public static void DropItemsAroundPosition(List<ItemEntity> items, Vector3 position) // Could be better!
    {
        Cell[] possibleLocations;
        int2 centre = WorldToGrid(position);

        int itemIndex = 0;

        Cell centerCell = GetGridCell(centre);

        if (centerCell.IsEmpty)
        {
            GameController.CreateEntity(items[itemIndex].GetType().ToString(), items[itemIndex].Name, items[itemIndex].Amount, centerCell.GridLocation, true);
            itemIndex++;
        }
        else if (centerCell.ContainsItem && centerCell.ItemContained.Name == items[itemIndex].Name && centerCell.ItemContained.MaxStackAmount >= (centerCell.ItemContained.Amount + items[itemIndex].Amount))
        {
            centerCell.ItemContained.Amount += items[itemIndex].Amount;
            centerCell.SendCellToOtherPlayers();
            itemIndex++;
            Debug.Log(centerCell.ItemContained.Amount);
        }


        if (itemIndex == items.Count) { return; }
        for (int i = 1; i < 20; i++)
        {
            possibleLocations = FindSquareOfCells(i, centre);
            foreach (var cell in possibleLocations)
            {
                if (cell.IsEmpty)
                {
                    GameController.CreateEntity("ItemEntity", items[itemIndex].Name, items[itemIndex].Amount, cell.GridLocation, true);
                    itemIndex++;
                }
                else if (cell.ContainsItem && cell.ItemContained.Name == items[itemIndex].Name && cell.ItemContained.MaxStackAmount >= (cell.ItemContained.Amount + items[itemIndex].Amount))
                {
                    cell.ItemContained.Amount += items[itemIndex].Amount;
                    cell.SendCellToOtherPlayers();
                    Debug.Log(cell.ItemContained.Amount);
                    itemIndex++;
                }

                if (itemIndex == items.Count) { return; }
            }
        }
    }

    //private static void PlaceItem(Cell cell, ItemEntity item)
    //{
    //    if (cell.ContainsItem && cell.ItemContained.Name == item.Name && cell.ItemContained.MaxStackAmount >= (cell.ItemContained.Amount + item.Amount))
    //    {
    //        cell.ItemContained.Amount += item.Amount;
    //        cell.SendCellToOtherPlayers();
    //    }
    //    else
    //    {
    //        GameController.CreateEntity("ItemEntity", item.Name, item.Amount, cell.GridLocation, true);
    //    }
    //}

    static public Cell FindAdjacentPassableCell(int2 position)
    {
        return Grid.Instance.FindAdjacentPassableCell(position);
    }
    static public Cell FindClosestAdjacentPassableCell(int2 position, int2 origin)
    {
        Cell[] adjacentCells = FindSquareOfCells(1, position);

       var orderedAdjacentCells = adjacentCells.OrderBy(cell => Vector2.Distance(new Vector2(origin.x, origin.y), cell.WorldPosition));

        foreach (var cell in orderedAdjacentCells)
        {
            if (!cell.Impassable)
            {
                return cell;
            }
        }
        return null;
    }

    static public Cell[] FindSquareOfCells(int radius, int2 centre)
    {
        List<Cell[]> cellArrays = new List<Cell[]>();
        if(radius > 0)
        {
            Cell[] cells = new Cell[radius * 8];
            //Top row
            Cell[] topRow = GetRowOfCells(new int2(centre.x - radius, centre.y + radius), new int2(centre.x + radius, centre.y + radius));

            //Bottom row
            Cell[] bottomRow = GetRowOfCells(new int2(centre.x - radius, centre.y - radius), new int2(centre.x + radius, centre.y - radius));

            //Left column
            Cell[] leftColumn = GetColumnOfCells(new int2(centre.x - radius, centre.y - (radius - 1)), new int2(centre.x - radius, centre.y + (radius - 1)));

            //Right column
            Cell[] rightColumn = GetColumnOfCells(new int2(centre.x + radius, centre.y - (radius - 1)), new int2(centre.x + radius, centre.y + (radius - 1)));

            topRow.CopyTo(cells, 0);
            bottomRow.CopyTo(cells, topRow.Length);
            leftColumn.CopyTo(cells, topRow.Length + bottomRow.Length);
            rightColumn.CopyTo(cells, topRow.Length + bottomRow.Length + leftColumn.Length);
            cellArrays.Add(cells);
            cellArrays.Add(FindSquareOfCells(radius - 1, centre));
        }
        else if(radius == 0)
        {
            Cell[] cells = new Cell[1];
            cells[0] = GetGridCell(centre);
            cellArrays.Add(cells);
        }
        return cellArrays.SelectMany(item => item).Distinct().ToArray();
    }   

    static public Cell[] FindSquareOfCells(int2 position1, int2 position2)
    {
        List<Cell> cellList = new List<Cell>();
        if(position1.y > position2.y)
        {
            if(position1.x > position2.x)
            {
                for (int x = position2.x; x < position1.x; x++)
                {
                    for (int y = position2.y; y < position1.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
            else
            {
                for (int x = position1.x; x < position2.x; x++)
                {
                    for (int y = position2.y; y < position1.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
        }
        else
        {
            if (position1.x > position2.x)
            {
                for (int x = position2.x; x < position1.x; x++)
                {
                    for (int y = position1.y; y < position2.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
            else
            {
                for (int x = position1.x; x < position2.x; x++)
                {
                    for (int y = position1.y; y < position2.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
        }

        return cellList.ToArray();
    }

    static public Cell[] FindSquareOfCells(Vector2 position1, Vector2 position2)
    {
        List<Cell> cellList = new List<Cell>();
        if (position1.y > position2.y)
        {
            if (position1.x > position2.x)
            {
                for (int x = (int)position2.x; x < position1.x; x++)
                {
                    for (int y = (int)position2.y; y < position1.y; y++)
                    {
                        if(!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
            else
            {
                for (int x = (int)position1.x; x < position2.x; x++)
                {
                    for (int y = (int)position2.y; y < position1.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
        }
        else
        {
            if (position1.x > position2.x)
            {
                for (int x = (int)position2.x; x < position1.x; x++)
                {
                    for (int y = (int)position1.y; y < position2.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
            else
            {
                for (int x = (int)position1.x; x < position2.x; x++)
                {
                    for (int y = (int)position1.y; y < position2.y; y++)
                    {
                        if (!IsValidLocation(new int2(x, y))) { continue; }
                        cellList.Add(GetGridCell(new int2(x, y)));
                    }
                }
            }
        }

        return cellList.ToArray();
    }

    private static Cell[] GetRowOfCells(int2 start, int2 end)
    {
        Debug.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0), Color.red, 50f);
        Cell[] cells = new Cell[end.x - start.x + 1];
        for (int i = 0; i < end.x - start.x + 1; i++)
        {
            if (IsValidLocation(new int2(start.x + i, start.y)))
            {
                cells[i] = GetGridCell(new int2(start.x + i, start.y));
            }             
        }
        return cells;
    }
    private static Cell[] GetColumnOfCells(int2 start, int2 end)
    {
        Debug.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0), Color.red, 50f);
        Cell[] cells = new Cell[end.y - start.y + 1];
        for (int i = 0; i < end.y - start.y + 1; i++)
        {
            if (IsValidLocation(new int2(start.x, start.y + i)))
            {
                cells[i] = GetGridCell(new int2(start.x, start.y + i));
            }
        }
        return cells;
    }
    public static bool IsValidLocation(int2 location)
    {
        if (location.x < 0 || location.y < 0 || location.x >= Grid.Instance.Width || location.y >= Grid.Instance.Height) { return false; }
        return true;
    }

    static public List<MoveableEntity> GetMoveableEntitiesAtGridPosition(int2 pos)
    {
        List<MoveableEntity> moveableEntities = new List<MoveableEntity>();
        Collider2D[] colliders = Physics2D.OverlapAreaAll(new Vector2(pos.x - 0.5f, pos.y - 0.5f), new Vector2(pos.x + 0.5f, pos.y + 0.5f));
        foreach (Collider2D collider in colliders)
        {
            MoveableEntity moveableEntity = collider.GetComponent<MoveableEntity>();
            if (moveableEntity != null)
            {
                moveableEntities.Add(moveableEntity);
            }
        }
        return moveableEntities;
    }

    static public Cell GetGridCell(Vector2 worldPosition)
    {
        if (!IsValidLocation(WorldToGrid(worldPosition))) { return null; }
        return Grid.Instance.grid[WorldToGrid(worldPosition).x, WorldToGrid(worldPosition).y];
    }
    static public Cell GetGridCell(int2 gridPosistion)
    {
        if (!IsValidLocation(gridPosistion)) { return null; }
        return Grid.Instance.grid[gridPosistion.x, gridPosistion.y];
    }
    static public int2 WorldToGrid(Vector2 worldPosition)
    {
        int2 gridLocation;
        gridLocation.x = Mathf.RoundToInt(worldPosition.x / Grid.Instance.sizeOfCells);
        gridLocation.y = Mathf.RoundToInt(worldPosition.y / Grid.Instance.sizeOfCells);
        return gridLocation;
    }

    static public Vector3 WorldToGridVector(Vector2 worldPosition)
    {
        Vector3Int gridLocation = new Vector3Int
        {
            x = Mathf.RoundToInt(worldPosition.x / Grid.Instance.sizeOfCells),
            y = Mathf.RoundToInt(worldPosition.y / Grid.Instance.sizeOfCells)
        };
        return gridLocation;
    }

    static public Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    static public int2 GetMouseGridPosition()
    {
        return WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    static public Vector2 GetMouseGridPositionVector()
    {
        return WorldToGridVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public static Cell GetCellAtMousePosition()
    {
        return GetGridCell(GetMouseGridPosition());
    }
    static public void DisplayPath(List<Cell> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].WorldPosition, path[i + 1].WorldPosition, Color.red, 100f);
        }
    }

    public static void UpdateCell(int2 gridLocation, string texturePath, bool passable, int itemAmount)
    {
        Cell cell = GetGridCell(gridLocation);

        bool sameTexture = texturePath == cell.TexturePath;

        cell.TexturePath = texturePath;
        if (cell.ContainsItem)
        {
            cell.ItemContained.Amount = itemAmount;
        }
        if (sameTexture) { return; }
        Grid.Instance.tileDictionary[gridLocation].sprite = Sprite.Create(cell.Texture,
                    new Rect(0, 0, cell.Texture.width, cell.Texture.height), // section of texture to use
                    new Vector2(0.5f, 0.5f), // pivot in centre
                    cell.Texture.width, // pixels per unity tile grid unit
                    1,
                    SpriteMeshType.Tight,
                    Vector4.zero
                );
    }

    //internal static void LoadGridFromMapData(string[] mapData, int width, int height)
    //{
    //    Cell[,] grid = new Cell[width, height]; ;
    //    Cell node;
    //    for (int i = 0; i < width; i++)
    //    {
    //        for (int j = 0; j < height; j++)
    //        {
    //            node = new Cell();
    //            Cell cellData = EntityDefinitions.Instance.CellDefinitions[mapData[i * height + j]];
    //            cellData.CopyProperties(node);
    //            node.DefaultPassable = !cellData.Impassable;
    //            node.GridLocation = new int2(i, j);
    //            node.WorldPosition = GridToWorld(i, j);
    //            grid[i, j] = node;
    //            //referenceGrid[i * height + j] = node.Name;
    //        }
    //    }
    //    Grid.Instance.grid = grid;
    //    GameController.GridReady();
    //}

    /// <summary>
    /// Converts the given x and y coordinates to a world position.
    /// </summary>
    /// <param name="x">X coordinates.</param>
    /// <param name="y">Y coordinates.</param>
    /// <returns></returns>
    public static Vector2 GridToWorld(int x, int y)
    {
        return new Vector2(x * Grid.Instance.sizeOfCells, y * Grid.Instance.sizeOfCells);
    }

    internal static bool CellIsImpassable(int2 position)
    {
        return Grid.Instance.GetGridCell(position).Impassable;
    } 
    internal static bool CellIsImpassable(Vector2 position)
    {
        return GetGridCell(position).Impassable;
    }

    internal static void BeginRender()
    {
        Grid.Instance.SetAllGridTextures();
    }
    public static Sprite CreateSprite(Texture2D texture, string pivotPoint, float scale)
    {
        string[] stringPivotPoints = pivotPoint.Split(',');
        float[] pivotPoints = { Mathf.Clamp01(float.Parse(stringPivotPoints[0])), Mathf.Clamp01(float.Parse(stringPivotPoints[1])) };
        return Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(pivotPoints[0], pivotPoints[1]),
            texture.width / scale
            );
    }

    public static int2 ToInt2(this Vector2 vector2)
    {
        return new int2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
    } 
    public static int2 ToInt2(this Vector3 vector3)
    {
        return new int2(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y));
    }
    public static Vector2 ToVector2(this int2 int2)
    {
        return new Vector2(int2.x, int2.y);
    }
}
