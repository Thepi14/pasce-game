using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static WallGraphicsGenerator;

public class Harpoon : MonoBehaviour
{
    public float speed = 5;
    public float force = 0;
    public bool shoot = false, returnToPlayer = false, returning, enteredWater = false, madeSplash = false;
    public GameObject player;
    private Player playerScript;
    private Rigidbody2D rb;
    public List<GameObject> fishList;
    public float harpRechargeTime = 0.7f;
    public float enterWaterTimeFish = 0.1f;
    private float enterWaterTimerFish = 0;
    private AudioSource audioSource;

    public class TouchedEvent : UnityEvent { }
    public TouchedEvent Touched;
    void Start()
    {
        fishList = new List<GameObject>();
        Touched = new TouchedEvent();
        rb = GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<Player>();
        playerScript.harpRechargeTime = harpRechargeTime;
        Touched.AddListener(playerScript.ReturnHarpoon);
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (!DetectFloorsNeighbours() && (shoot == true || returning == true) && GetComponent<SpriteRenderer>().enabled && enterWaterTimerFish > enterWaterTimeFish)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Fish";
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponent<WaveEffect>().run = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Moveable";
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponent<WaveEffect>().run = false;
        }
        if (shoot || returning)
            enterWaterTimerFish += Time.deltaTime;
        else
            enterWaterTimerFish = 0;
        if ((returning || shoot) && enterWaterTimerFish > enterWaterTimeFish - 0.01f && !DetectFloorsNeighbours())
        {
            enteredWater = true;
        }
        else
        {
            enteredWater = false;
            madeSplash = false;
        }
        if (!madeSplash && enteredWater)
        {
            madeSplash = true;
            audioSource.Play();
        }
    }
    private void FixedUpdate()
    {
        rb.angularVelocity = 0;
        //if (gameObject.layer = )
        if (shoot && !returning)
        {
            if (DetectFloorsNeighbours())
            {
                gameObject.layer = 13;
            }
            else if (enterWaterTimerFish > enterWaterTimeFish)
            {
                gameObject.layer = 11;
            }
            rb.velocity = transform.up * (speed + (playerScript.harpCounter * speed));
        }
        else if (!shoot && returning)
        {
            gameObject.layer = 13;
            float v = Mathf.Atan2(transform.position.y - player.transform.position.y, transform.position.x - player.transform.position.x);
            gameObject.transform.eulerAngles = new Vector3(0, 0, (Mathf.Rad2Deg * v) - 90);
            rb.velocity = -transform.up * speed * 0.75f;
            if (Vector2.Distance(transform.position, player.transform.position) < 1.5f)
            {
                returning = false;
                playerScript.harpLaunched = false;
                //playerScript.harpCounter = 0;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, player.transform.position) < 1.5f)
            {
                returning = false;
                playerScript.harpLaunched = false;
                //playerScript.harpCounter = 0;
            }
            //rb.velocity = Vector2.zero;
            gameObject.layer = 13;
        }
    }
    public bool DetectFloorsNeighbours()
    {
        for (int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (CheckIfInsideArrayLimits(publicGradientMap, (int)transform.position.x + i, (int)transform.position.y + j) && publicWhiteMap.GetPixel((int)transform.position.x + i, (int)transform.position.y + j) == Color.white)
                    return true;
            }
        }
        return false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10 && gameObject.layer == 11)
            fishList.Add(collision.gameObject);
        Touched.Invoke();
        returning = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Touched.Invoke();
        returning = true;
    }
}
