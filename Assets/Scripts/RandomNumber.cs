using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomNumber : MonoBehaviour
{
    int randomNumberIndex = 0;
    int channel = -1;
    private Texture2D texture;
    private Vector2Int randomNumberIndex2D { get => new Vector2Int(randomNumberIndex % 100, randomNumberIndex / 100); }

    [HideInInspector] public static RandomNumber Instance { get; private set; }

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
    public static int Get()
    {
        if (Instance == null) { return -1; }
        if (Instance.texture == null)
        {
            byte[] fileData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Coconut.png"));
            Instance.texture = new Texture2D(2,2);
            Instance.texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        Instance.channel++;
        switch (Instance.channel)
        {
            case 0:
                return (int)(Instance.texture.GetPixel(Instance.randomNumberIndex2D.x, Instance.randomNumberIndex2D.y).r * 100);
            case 1:
                return (int)(Instance.texture.GetPixel(Instance.randomNumberIndex2D.x, Instance.randomNumberIndex2D.y).g * 100);
            case 2:
                Instance.channel = -1;
                Instance.randomNumberIndex++;
                return (int)(Instance.texture.GetPixel(Instance.randomNumberIndex2D.x, Instance.randomNumberIndex2D.y).b * 100);
        }
        Debug.LogError("Random Number Gone Weird!");
        return -1;
    }

    public static bool CoinFlip()
    {
        if(Get() >= 50)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
