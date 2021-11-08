using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityToolbag;

public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    private Queue<Tuple<int2, int2, float>> pathfindingRequests = new Queue<Tuple<int2, int2, float>>();
    private bool processingPathfinding;

    public int sizeOfCells;
    public Cell[,] grid;
    public Texture2D testTexture;
    public Texture2D testTexture2;
    public Tilemap tilemap;
    public Dictionary<int2, Tile> tileDictionary = new Dictionary<int2, Tile>();

    public int Width { get => width; }
    public int Height { get => height; }
    [HideInInspector] public static Grid Instance { get; private set; }
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
    private void Start()
    {
        width = GridGenerator.Instance.width;
        height = GridGenerator.Instance.height;
        grid = GridGenerator.Instance.grid;
        GameController.GridReady();
    }

    private void Update()
    {
        if(!processingPathfinding && pathfindingRequests.Count != 0)
        {
            var tuple = pathfindingRequests.Dequeue();
            Future<List<Cell>> futureCells = FindPathAsyncronous(tuple.Item1, tuple.Item2);
            futureCells.OnSuccess((value) =>
            {
                PathfindingBuffer.AddPath(tuple.Item3, value.value);
                processingPathfinding = false;
            });
        }
    }
    private void GenerateEntities()
    {
        System.Random rnd = new System.Random();
        var temp = EntityDefinitions.Instance.EntitiesByTypes["StructureEntity"];
        var list = temp.Values.Cast<StructureEntity>().ToList();
        var list2 = list.Where(kvp => (kvp).Commonness > 0).ToList();
        list2 = list2.OrderBy(x => x.Commonness).ToList();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int random = rnd.Next(100);

                foreach (var item in list2)
                {
                    if (random <= item.Commonness) { GameController.CreateEntity("StructureEntity", item.Name, new int2(i, j), true); break; }
                }

            }
        }
    }

    public void SetAllGridTextures()
    {
        if (RoomData.RoomCreator)
        {
            GenerateEntities();
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
                Texture2D texture = grid[i, j].Texture;
                tile.sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height), // section of texture to use
                    new Vector2(0.5f, 0.5f), // pivot in centre
                    texture.width, // pixels per unity tile grid unit
                    1,
                    SpriteMeshType.Tight,
                    Vector4.zero
                );
                tilemap.SetTile(new Vector3Int(i, j, 0), tile);
                tileDictionary.Add(new int2(i, j), tile);

            }
        }
    }



    /// <summary>
    /// Finds the 'shortest' path between two grid positions heuristically.
    /// </summary>
    /// <param name="start">Start position.</param>
    /// <param name="end">End position.</param>
    /// <returns>Path of shortest distance in list form.</returns>
    public void FindPath(int2 start, int2 end, float key)
    {
        var tuple = new Tuple<int2, int2, float>(start, end, key);
        pathfindingRequests.Enqueue(tuple);      
    }

    private Future<List<Cell>> FindPathAsyncronous(int2 start, int2 end)
    {
        processingPathfinding = true;
        Future<List<Cell>> future = new Future<List<Cell>>();
        // this will use a thread from a thread pool
        future.Process(() =>
        {

            List<Cell> startCellAsAList = new List<Cell>();

            if (end.x < 0 || end.x >= width || end.y < 0 || end.y >= height) { return startCellAsAList; }

            if (grid[end.x, end.y].Impassable || start.Equals(end))
            {
                startCellAsAList.Add(grid[start.x, start.y]);
                return startCellAsAList;
            }

            List<Cell> openList = new List<Cell>();
            List<Cell> closedList = new List<Cell>();
            List<Cell> gCheckList;

            List<Cell> nextCells;
            List<Cell> pathCellList = new List<Cell>();
            Cell currentCell = grid[start.x, start.y];
            currentCell.gCost = 0f;

            int count = 0;
            do
            {
                count++;
                if (count > 5000) { Debug.LogError("Pathfinding loop has gone infinite or is ridiculously long."); break; }

                nextCells = FindAdjacentCells(currentCell.GridLocation, closedList, openList);

                openList.Remove(currentCell);
                closedList.Add(currentCell);

                for (int i = 0; i < nextCells.Count; i++)
                {
                    CalculateGCost(nextCells[i]);
                    nextCells[i].hCost = FindGridDistance(nextCells[i].GridLocation.x, nextCells[i].GridLocation.y, end.x, end.y);
                    openList.Add(nextCells[i]);
                }

                float lowestFCost = float.MaxValue;
                Cell fastestCell = null;
                for (int i = 0; i < openList.Count; i++)
                {

                    if (openList[i].fCost < lowestFCost)
                    {
                        fastestCell = openList[i];
                        lowestFCost = fastestCell.fCost;
                    }
                    if (openList[i].GridLocation.Equals(end))
                    {
                        lowestFCost = 0f;
                        fastestCell = openList[i];
                    }
                }

                currentCell = fastestCell;
                try
                {
                    gCheckList = FindAdjacentCells(currentCell.GridLocation, closedList, openList);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return null;
                    //throw;
                }



                for (int i = 0; i < gCheckList.Count; i++)
                {
                    if (currentCell.gCost + gCheckList[i].gCost < gCheckList[i].gCost)
                    {
                        gCheckList[i].previousCell = currentCell;
                        CalculateGCost(gCheckList[i]);
                    }
                }

            } while ((currentCell.GridLocation.x != end.x || currentCell.GridLocation.y != end.y) && closedList.Count + openList.Count != width * height);

            bool startCellFound = false;
            grid[start.x, start.y].previousCell = null;

            int count2 = 0;
            currentCell = grid[end.x, end.y];
            while (!startCellFound)
            {
                count2++;
                if (count2 > 500) { Debug.LogError("Path recollection loop has gone infinite"); break; }

                if(currentCell.previousCell != null && currentCell.previousCell.previousCell != currentCell)
                {
                    pathCellList.Add(currentCell);
                }
         

                if (currentCell.previousCell == currentCell) { Debug.Log("OH GOD OH WHY"); }

                if (currentCell.previousCell != null)
                {
                    currentCell = currentCell.previousCell;

                }
                else
                {
                    startCellFound = true;
                }

            }
            pathCellList.Reverse();
            if (pathCellList.Count >= 3)
            {
                //pathCellList.RemoveAt(0);
            }
            return pathCellList;
        });
        return future;
    }

    /// <summary>
    /// Find an adjacent node which can me walked through.
    /// </summary>
    /// <param name="startLocation">Middle node to have checks be commenced around.</param>
    /// <returns></returns>
    public Cell FindAdjacentPassableCell(int2 startLocation)
    {
        Cell currentCell = null;
        if (startLocation.x != 0)
        {
            currentCell = GetGridCell(new int2(startLocation.x - 1, startLocation.y));
            if (!currentCell.Impassable) { return currentCell; }

            if (startLocation.y != 0)
            {
                currentCell = GetGridCell(new int2(startLocation.x - 1, startLocation.y - 1));
                if (!currentCell.Impassable) { return currentCell; }
            }
        }
        if (startLocation.x != width - 1)
        {
            currentCell = GetGridCell(new int2(startLocation.x + 1, startLocation.y));
            if (!currentCell.Impassable) { return currentCell; }

            if (startLocation.y != height - 1)
            {
                currentCell = GetGridCell(new int2(startLocation.x + 1, startLocation.y + 1));
                if (!currentCell.Impassable) { return currentCell; }
            }
        }

        if (startLocation.y != 0)
        {
            currentCell = GetGridCell(new int2(startLocation.x, startLocation.y - 1));
            if (!currentCell.Impassable) { return currentCell; }

            if (startLocation.x != width - 1)
            {
                currentCell = GetGridCell(new int2(startLocation.x + 1, startLocation.y - 1));
                if (!currentCell.Impassable) { return currentCell; }
            }
        }
        if (startLocation.y != height - 1)
        {
            currentCell = GetGridCell(new int2(startLocation.x, startLocation.y + 1));
            if (!currentCell.Impassable) { return currentCell; }

            if (startLocation.x != 0)
            {
                currentCell = GetGridCell(new int2(startLocation.x - 1, startLocation.y));
                if (!currentCell.Impassable) { return currentCell; }
            }
        }

        return null;
    }

   
    /// <summary>
    /// Finds and returns the adjecent nodes if adjecent nodes exist
    /// </summary>
    /// <param name="currentCellLocation">Location of middle node.</param>
    /// <param name="closedList">A* closed list.</param>
    /// <param name="openList">A* open list.</param>
    /// <returns></returns>
    private List<Cell> FindAdjacentCells(int2 currentCellLocation, List<Cell> closedList, List<Cell> openList)
    {
        List<Cell> nextCells = new List<Cell>();

        int2[,] nextint2s = new int2[3, 3];

        //Right top      
        nextint2s[2, 2] = new int2(currentCellLocation.x + 1, currentCellLocation.y + 1);

        //Right middle    
        nextint2s[2, 1] = new int2(currentCellLocation.x + 1, currentCellLocation.y);

        //Right bottom
        nextint2s[2, 0] = new int2(currentCellLocation.x + 1, currentCellLocation.y - 1);

        //Middle top
        nextint2s[1, 2] = new int2(currentCellLocation.x, currentCellLocation.y + 1);

        //Middle bottom
        nextint2s[1, 0] = new int2(currentCellLocation.x, currentCellLocation.y - 1);

        //Left top
        nextint2s[0, 2] = new int2(currentCellLocation.x - 1, currentCellLocation.y + 1);

        //Left middle
        nextint2s[0, 1] = new int2(currentCellLocation.x - 1, currentCellLocation.y);

        //Left bottom
        nextint2s[0, 0] = new int2(currentCellLocation.x - 1, currentCellLocation.y - 1);

        //Middle middle
        nextint2s[1, 1] = currentCellLocation;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (!GridUtility.IsValidLocation(nextint2s[i, j]))
                {
                    nextint2s[i, j] = new int2(-1, -1);
                }
            }
        }


        if (nextint2s[2, 1].x != -1 && GetGridCell(nextint2s[2, 1]).Impassable )
        {
            nextint2s[2, 2] = new int2(-1, -1);
            nextint2s[2, 0] = new int2(-1, -1);
        }
        if (nextint2s[1, 2].x != -1 && GetGridCell(nextint2s[1, 2]).Impassable)
        {
            nextint2s[0, 2] = new int2(-1, -1);
            nextint2s[2, 2] = new int2(-1, -1);
        }
        if (nextint2s[1, 0].x != -1 && GetGridCell(nextint2s[1, 0]).Impassable)
        {
            nextint2s[0, 0] = new int2(-1, -1);
            nextint2s[2, 0] = new int2(-1, -1);
        }
        if (nextint2s[0, 1].x != -1 && GetGridCell(nextint2s[0, 1]).Impassable)
        {
            nextint2s[0, 0] = new int2(-1, -1);
            nextint2s[0, 2] = new int2(-1, -1);
        }


        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (nextint2s[i, j].x != -1)
                {
                    //If the node isn't in the closed nor open list, and it isn't impassible terrain:
                    if (!closedList.Contains(GetGridCell(nextint2s[i, j])) && !openList.Contains(GetGridCell(nextint2s[i, j])) && !GetGridCell(nextint2s[i, j]).Impassable)
                    {
                        //Nor if it isn't the given node:
                        if (GetGridCell(nextint2s[i, j]) != GetGridCell(currentCellLocation))
                        {
                            //Set the next node's previous node as the one it came from.
                            GetGridCell(nextint2s[i, j]).previousCell = GetGridCell(currentCellLocation);
                            nextCells.Add(GetGridCell(nextint2s[i, j]));
                        }
                    }
                }
            }
        }


        return nextCells;
    }

    /// <summary>
    /// Calculated the Gcost of a given node.
    /// </summary>
    /// <param name="nodeToCalculate">Given node to calculate Gcost of.</param>
    private void CalculateGCost(Cell nodeToCalculate)
    {
        if (nodeToCalculate.GridLocation.x == nodeToCalculate.previousCell.GridLocation.x || nodeToCalculate.GridLocation.y == nodeToCalculate.previousCell.GridLocation.y)
        {
            nodeToCalculate.gCost = 1f;
        }
        else
        {
            nodeToCalculate.gCost = Mathf.Sqrt(2);
        }
        nodeToCalculate.gCost += nodeToCalculate.previousCell.gCost;

    }

    /// <summary>
    /// Finds the distance between two nodes.
    /// </summary>
    /// <param name="xStart"></param>
    /// <param name="yStart"></param>
    /// <param name="xEnd"></param>
    /// <param name="yEnd"></param>
    /// <returns></returns>
    public float FindGridDistance(int xStart, int yStart, int xEnd, int yEnd)
    {
        return Mathf.Sqrt(Mathf.Pow(Mathf.Abs(xEnd - xStart), 2f) + Mathf.Pow(Mathf.Abs(yEnd - yStart), 2f));
    }


    /// <summary>
    /// Gets the grid node in the given position.
    /// </summary>
    /// <param name="gridLocation">The grid location of the node.</param>
    /// <returns></returns>
    public Cell GetGridCell(int2 gridLocation)
    {
        if (!GridUtility.IsValidLocation(gridLocation)) { return null; }
        return grid[gridLocation.x, gridLocation.y];
    }


    /// <summary>
    /// Converts the given <see cref="int2"/> to a world position.
    /// </summary>
    /// <param name="gridLocation">Grid loctation to convert.</param>
    /// <returns></returns>
    public Vector2 GridToWorld(int2 gridLocation)
    {
        return new Vector2(gridLocation.x * sizeOfCells, gridLocation.y * sizeOfCells);
    }

    /// <summary>
    /// Converts the given x and y coordinates to a world position.
    /// </summary>
    /// <param name="x">X coordinates.</param>
    /// <param name="y">Y coordinates.</param>
    /// <returns></returns>
    public Vector2 GridToWorld(int x, int y)
    {
        return new Vector2(x * sizeOfCells, y * sizeOfCells);
    }
}
