using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject player, harpoon;
    void Start()
    {
        
    }
    void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1;
        transform.position = player.transform.position + (harpoon.transform.position - player.transform.position) / 2;
        float v = Mathf.Atan2(harpoon.transform.position.y - player.transform.position.y, harpoon.transform.position.x - player.transform.position.x);
        gameObject.transform.eulerAngles = new Vector3(0, 0, (Mathf.Rad2Deg * v) + 90);
        GetComponent<SpriteRenderer>().size = new(0.19f, Vector2.Distance(player.transform.position, harpoon.transform.position));
    }
}
