using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineLol : MonoBehaviour
{
    public GameObject target, playerObj;
    private GameObject cameraObj;
    public float velocityX, velocityY;
    private float prevX, prevY;
    private Camera cam;
    public float minSize, maxSize;
    private Player player;
    public PlayerUI playerUI;

    void Start()
    {
        //playerUI = transform.parent.Find("Canvas").GetComponent<PlayerUI>();
        player = playerObj.GetComponent<Player>();
        minSize = 1.5f;
        maxSize = 15;
        cameraObj = gameObject;
        cam = cameraObj.GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0 && cam.orthographicSize > minSize && playerUI.mouseOnUI == false && !playerUI.seeingMap)
        {
            cam.orthographicSize -= 0.4f;
        }
        else if (Input.mouseScrollDelta.y < 0 && cam.orthographicSize < maxSize && playerUI.mouseOnUI == false && !playerUI.seeingMap)
        {
            cam.orthographicSize += 0.4f;
        }
        if (cam.orthographicSize <= minSize)
            cam.orthographicSize = minSize;
        else if (cam.orthographicSize >= maxSize)
            cam.orthographicSize = maxSize;
    }
    private void FixedUpdate()
    {
        if (target != playerObj)
            cameraObj.transform.position = new Vector3(target.transform.position.x, target.transform.position.y - player.FX.yForCamera, cameraObj.transform.position.z);
        else
            cameraObj.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, cameraObj.transform.position.z);

        velocityX = target.transform.position.x - prevX;
        velocityY = target.transform.position.y - prevY;
        prevX = target.transform.position.x;
        prevY = target.transform.position.y;
    }
}
