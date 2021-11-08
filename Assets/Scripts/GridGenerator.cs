using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbag;

public class GridGenerator : MonoBehaviour
{
    public Cell[,] grid;
    public int width;
    public int height;
    public UnityEngine.UI.Slider widthSlider;
    public UnityEngine.UI.Slider heightSlider;
    private string[] referenceGrid;
    [HideInInspector] public static GridGenerator Instance { get; private set; } //Singleton structure.
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
    public void GenerateGrid(int seed)
    {
        width = (int)widthSlider.value;
        height = (int)heightSlider.value;
        Future<Cell[,]> futureCells = GenerateCells(width, height, seed);
        futureCells.OnSuccess((value) => OnComplete(value.value));
      
    }
    private void OnComplete(Cell[,] value)
    {
        grid = value;
        Debug.Log("Loading scene");
        SceneManager.LoadScene("Game");
    }
    private Future<Cell[,]> GenerateCells(int width, int height, int seed)
    {
        Future<Cell[,]> future = new Future<Cell[,]>();

        // this will use a thread from a thread pool
        future.Process(() =>
        {
            Cell[,] nodeArray = new Cell[width, height];
            referenceGrid = new string[width * height];
            Cell node;
            List<string> keyList = new List<string>(EntityDefinitions.Instance.CellDefinitions.Keys);
            System.Random rnd = new System.Random(seed);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    node = new Cell();
                    string randomKey = keyList[rnd.Next(keyList.Count)];
                    Cell randomCell;
                    if (rnd.Next(3) == 1)
                    {
                        randomCell = EntityDefinitions.Instance.CellDefinitions[randomKey];
                    }
                    else
                    {
                        randomCell = EntityDefinitions.Instance.CellDefinitions["Grass"];
                    }
                    randomCell.CopyProperties(node);
                    node.DefaultPassable = !randomCell.Impassable;
                    node.GridLocation = new int2(i, j);
                    node.WorldPosition = new Vector2(i, j);
                    nodeArray[i, j] = node;
                    referenceGrid[i * height + j] = node.Name;

                }
            }
            return nodeArray;
        });

        return future;

    }


}
