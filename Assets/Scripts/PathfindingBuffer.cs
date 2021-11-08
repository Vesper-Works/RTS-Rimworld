using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingBuffer : MonoBehaviour
{
    public Dictionary<float, List<Cell>> Paths = new Dictionary<float, List<Cell>>();

    private static PathfindingBuffer Instance { get; set; } //Singleton structure.

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
    public static void AddPath(float key, List<Cell> path)
    {
        Instance.Paths.Add(key, path);
        Debug.Log("Added path: " + key);
    }
    public static List<Cell> GetPath(float key)
    {
        if (Instance.Paths.ContainsKey(key))
        {
            Debug.Log("Retrieved path: " + key);
            return Instance.Paths[key];          
        }
        return null;
    }

}
