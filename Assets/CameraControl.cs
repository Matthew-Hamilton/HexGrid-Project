using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector2 mousePosition;

    Camera thisCamera;

    void Start()
    {
        thisCamera = this.GetComponent<Camera>();
        mousePosition = MousePos();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            Vector2 mouseChange = mousePosition - (Vector2)MousePos();
            transform.position += new Vector3(mouseChange.x, mouseChange.y, 0);
        }

        if(Input.mouseScrollDelta.y < 0 && thisCamera.orthographicSize < 300)
        {
            thisCamera.orthographicSize -= Input.mouseScrollDelta.y * 2;
        }
        if (Input.mouseScrollDelta.y > 0 && thisCamera.orthographicSize > 2)
        {
            thisCamera.orthographicSize -= Input.mouseScrollDelta.y * 2;
        }

        if (thisCamera.orthographicSize <= 0)
            thisCamera.orthographicSize = 2;

        mousePosition = MousePos();
    }

    Vector3 MousePos()
    {
        return thisCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}


