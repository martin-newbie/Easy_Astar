using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    public float moveSpeed;

    Camera cam;
    float camUnitSize => 16f / 9f;

    private void Update()
    {
        cam = GetComponent<Camera>();
        var mouse = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        Vector2 moveDir = new Vector2();

        if (mouse.x <= 0.2f) moveDir.x = -1;
        else if (mouse.x >= 0.8f) moveDir.x = 1;

        if (mouse.y <= 0.2f) moveDir.y = -1;
        else if (mouse.y >= 0.8f) moveDir.y = 1;


        moveDir *= moveSpeed * Time.deltaTime;


        if (UnitManager.Instance.IsMaxOutBoundX(transform.position.x + moveDir.x, cam.orthographicSize * camUnitSize) && moveDir.x > 0)
        {
            moveDir.x = 0f;
        }
        if (UnitManager.Instance.IsMinOutBoundX(transform.position.x + moveDir.x, cam.orthographicSize * camUnitSize) && moveDir.x < 0)
        {
            moveDir.x = 0f;
        }
        if (UnitManager.Instance.IsMaxOutBoundY(transform.position.y + moveDir.y, cam.orthographicSize) && moveDir.y > 0)
        {
            moveDir.y = 0f;
        }
        if(UnitManager.Instance.IsMinOutBoundY(transform.position.y + moveDir.y, cam.orthographicSize) && moveDir.y < 0)
        {
            moveDir.y = 0f;
        }

        transform.Translate(moveDir);
    }
}
