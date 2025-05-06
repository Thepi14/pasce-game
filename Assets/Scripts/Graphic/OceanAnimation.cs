using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TextureFunction;

public class OceanAnimation : MonoBehaviour
{
    [SerializeField] private GameObject OceanImage, player;
    private float timer, counter;
    private Vector2 dir;
    private float distance, speed, textureMoveSpeed;
    public int velX, velY;
    void Start()
    {
        speed = player.GetComponent<Player>().speed;
        timer = 0.07f;
        textureMoveSpeed = 12;
        distance = 2;
    }
    void Update()
    {
        dir = player.GetComponent<Rigidbody2D>().velocity;
        counter += Time.deltaTime;
        if (counter >= timer)
        {
            counter = 0f;
            OceanImage.GetComponent<SpriteRenderer>().sprite = MovePixelsInSprite(OceanImage.GetComponent<SpriteRenderer>().sprite, 1, 1);
        }
    }
    private void FixedUpdate()
    {
        if (player.transform.position.x >= OceanImage.transform.position.x + distance || player.transform.position.x <= OceanImage.transform.position.x - distance)
            OceanImage.transform.position = new Vector3(player.transform.position.x, OceanImage.transform.position.y, OceanImage.transform.position.z);

        if (player.transform.position.y >= OceanImage.transform.position.y + distance || player.transform.position.y <= OceanImage.transform.position.y - distance)
            OceanImage.transform.position = new Vector3(OceanImage.transform.position.x, player.transform.position.y, OceanImage.transform.position.z);
    }
}
