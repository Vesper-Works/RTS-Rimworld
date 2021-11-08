using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessageCreator : MonoBehaviour
{
    public GameObject FloatingMessageGameobject;
    private static GameMessageCreator Instance { get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static void CreateFloatingText(string message, Vector2 position)
    {
        Instantiate(Instance.FloatingMessageGameobject, position, Quaternion.identity, null)
            .GetComponent<TMPro.TextMeshPro>().text = message;
    }

}
