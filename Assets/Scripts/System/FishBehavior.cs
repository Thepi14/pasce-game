using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallGraphicsGenerator;

public class FishBehavior : MonoBehaviour
{
    public Vector2 target;
    public float speed = 4;
    public float hitPoints = 10f;
    public int type;
    private int value;
    private Rigidbody2D rb;
    private WaitConditionRef<Vector2> mover;
    private float timer = 2f, counter = 0, r, harpoonInitialZAngle;
    public GameObject player;
    private GameObject harpoon;
    public FishGenerator generator;
    public bool fisched = false;
    public string fishName = string.Empty;
    public int meatQuantity = 1;
    public Sprite fishIcon;

    private void Start()
    {
        fisched = false;
        rb = GetComponent<Rigidbody2D>();
        target = new Vector2(0, 0);
    }
    public void SetFish()
    {
        counter = timer;
        StartCoroutine(fadeIn());
        fishIcon = generator.fishIcons[type - 1];
        if (type == 1)
        {
            value = 10;
            fishName = "Peixe Comum";
            meatQuantity = 1;
        }
        else if (type == 2)
        {
            value = 25;
            fishName = "Peixe Palhaço";
            meatQuantity = 1;
        }
        else if (type == 3)
        {
            value = 500;
            fishName = "Peixe Dourado";
            meatQuantity = 1;
        }
        else if (type == 4)
        {
            value = 15;
            meatQuantity = 1;
        }
        else
        {
            value = 0;
            fishName = "Peixe Dourado";
            meatQuantity = 0;
        }
    }
    void Update()
    {
        if (!fisched)
            counter += Time.deltaTime;

        target = new Vector2(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4));
        if (CheckIfInsideArrayLimits(publicGradientMap, (int)(transform.position.x + target.x), (int)(transform.position.y + target.y)))
            r = publicGradientMap.GetPixel((int)(transform.position.x + target.x), (int)(transform.position.y + target.y)).r;
        target *= 0.2f;

        if (counter >= timer && publicGradientMap.GetPixel((int)transform.position.x, (int)transform.position.y).r >= r)
        {
            rb.velocity = new Vector2(target.x, target.y);
            counter = 0;
        }
        if (!fisched)
            Flip();
        if (player != null)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < 10 && (player.GetComponent<Player>().boat.GetComponent<Rigidbody2D>().velocity != Vector2.zero || player.GetComponent<Rigidbody2D>().velocity != Vector2.zero))
            {
                var dir = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - player.transform.position.y, transform.position.x - player.transform.position.x), Vector3.forward) * Vector3.right;
                rb.velocity = dir * 2.5f;
            }
            else if (Vector2.Distance(transform.position, player.transform.position) > 45)
            {
                gameObject.AddComponent<TimerDestroy>();
            }
        }
        if (fisched)
        {
            GetComponent<SpriteRenderer>().color = harpoon.GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().sortingOrder = harpoon.GetComponent<SpriteRenderer>().sortingOrder + 1;
            GetComponent<SpriteRenderer>().sortingLayerName = harpoon.GetComponent<SpriteRenderer>().sortingLayerName;
        }
    }
    public void FixedUpdate()
    {
        rb.angularVelocity = 0;
        if(fisched)
        {
            transform.position = harpoon.transform.position + (harpoon.transform.up * 0.08f);
            if (transform.eulerAngles.y == 0)
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, harpoon.transform.eulerAngles.z - harpoonInitialZAngle);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, -harpoon.transform.eulerAngles.z + harpoonInitialZAngle);
            }
            if (harpoon.GetComponent<SpriteRenderer>().enabled == false || harpoon.GetComponent<Harpoon>().returning == false)
                Destroy(gameObject);
        }
    }
    private void Flip()
    {
        if (rb.velocity.x <= 0) { transform.rotation = Quaternion.Euler(0, 0, 0); }
        else if (rb.velocity.x > 0) { transform.rotation = Quaternion.Euler(0, 180, 0); }
    }
    IEnumerator fadeIn()
    {
        var c = gameObject.GetComponent<SpriteRenderer>().color;
        for (float i = 0; i <= 1; i += 0.05f)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, i);
            yield return new WaitForSeconds(0.05f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        hitPoints = 0;
        if(collision.gameObject.layer == 11 && hitPoints == 0 && !fisched)
        {
            harpoon = collision.gameObject;
            harpoonInitialZAngle = harpoon.transform.eulerAngles.z;
            GetComponent<CircleCollider2D>().enabled = false;
            fisched = true;
            //Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        generator.fishList.Remove(gameObject);
        if (harpoon != null && harpoon.GetComponent<Harpoon>().fishList.Contains(gameObject))
            harpoon.GetComponent<Harpoon>().fishList.Remove(gameObject);
        if (fisched)
        {
            InventoryUI inventoryUI = player.GetComponent<Player>().playerUI.inventoryUI;
            inventoryUI.AddItem(new Item(fishName, fishIcon, value, 16, 1));
        }
    }
}
