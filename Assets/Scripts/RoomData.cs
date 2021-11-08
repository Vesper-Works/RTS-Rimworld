using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    private bool roomCreator;

    public static bool RoomCreator { get => Instance.roomCreator; set => Instance.roomCreator = value; }

    [HideInInspector] public static RoomData Instance { get; private set; } //Singleton structure.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
