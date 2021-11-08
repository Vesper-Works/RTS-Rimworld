using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatUpThenDisappear : MonoBehaviour
{
    private void Start()
    {
        Invoke("DestroyThis", 2);
    }
    void Update()
    {
        transform.position += new Vector3(0, 1 * Time.deltaTime, 0);
    }
    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
