using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float startCameraSize = 20;
    private float cameraSize;
    [SerializeField] private float cameraSizeMax = 60;
    [SerializeField] private float cameraSizeMin = 5;

    [SerializeField] private float scrollSpeed = 5;
    [SerializeField] private float slowMoveSpeed = 5;
    [SerializeField] private float fastMoveSpeed = 20;

    private float moveSpeed;
    private bool draggingCamera;
    private Vector3 lockPosition;
    void Start()
    {
        cameraSize = startCameraSize;
        moveSpeed = slowMoveSpeed;
    }
    void Update()
    {
        KeyMovement();
        DragMovement();
    }

    private void KeyMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { moveSpeed = fastMoveSpeed; }
        else { moveSpeed = slowMoveSpeed; }
        moveSpeed *= Time.deltaTime;

        if (Input.GetKey("a"))
        {
            transform.position = new Vector3(transform.position.x - moveSpeed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey("w"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed, transform.position.z);
        }
        if (Input.GetKey("d"))
        {
            transform.position = new Vector3(transform.position.x + moveSpeed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey("s"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed, transform.position.z);
        }

        cameraSize += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;

        if (cameraSize <= cameraSizeMin)
        {
            cameraSize = cameraSizeMin + 0.00001f;
        }
        else if (cameraSize >= cameraSizeMax)
        {
            cameraSize = cameraSizeMax - 0.00001f;
        }
        GetComponent<Camera>().orthographicSize = cameraSize;
    }

    private void DragMovement()
    {
        if (!Input.GetMouseButton(2)) { draggingCamera = false; return; }

        if (!draggingCamera) { lockPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); }

        transform.position = transform.position + (lockPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition));
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        draggingCamera = true;
    }
}