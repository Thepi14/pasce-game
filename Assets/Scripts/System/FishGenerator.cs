using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallGraphicsGenerator;

public class FishGenerator : MonoBehaviour
{
    public GameObject player, oceanOverlap;
    public Sprite[] fishs;
    public Sprite[] fishIcons;
    public float counter, timer = 10f;
    public List<GameObject> fishList;
    public Color oceanSinkColor;

    void Start()
    {
        oceanSinkColor = oceanOverlap.GetComponent<SpriteRenderer>().color;
        oceanSinkColor = new Color(oceanSinkColor.r, oceanSinkColor.g, oceanSinkColor.b, 1f);
        WallGraphicsGenerator.oceanSinkColor = oceanSinkColor;
    }
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > timer)
        {
            //print(WallGraphicsGenerator.oceanSinkColor);
            counter = 0;
            for (int x = -100; x <= 100; x++)
            {
                for (int y = -100; y <= 100; y++)
                {
                    if (CheckIfInsideArrayLimits(publicWhiteMap, (int)player.transform.position.x + x, (int)player.transform.position.y + y))
                    {
                        if (publicGradientMap.GetPixel((int)player.transform.position.x + x, (int)player.transform.position.y + y).r < 0.7f && UnityEngine.Random.Range(-80, 50) > 48 && Vector2.Distance(player.transform.position, new Vector2((int)player.transform.position.x + x, (int)player.transform.position.y + y)) < 25 && fishList.Count < 20)
                        {
                            GameObject fish = new GameObject("0");
                            fish.transform.parent = transform;
                            fish.transform.position = new Vector3(player.transform.position.x + x, player.transform.position.y + y, 0);
                            fish.layer = 10;
                            fish.AddComponent<SpriteRenderer>();
                            var a = fish.GetComponent<SpriteRenderer>();
                            var rand = UnityEngine.Random.Range(0, 2);
                            fish.name = "fish" + rand;
                            a.sprite = fishs[rand];
                            a.sortingLayerName = "Fish";
                            a.color = (Color.white / 2) + (oceanOverlap.GetComponent<SpriteRenderer>().color / 2);
                            a.color = new Color(a.color.r, a.color.g, a.color.b, 0);
                            fish.AddComponent<Rigidbody2D>();
                            fish.GetComponent<Rigidbody2D>().freezeRotation = true;
                            fish.AddComponent<CircleCollider2D>();
                            fish.GetComponent<CircleCollider2D>().radius = 0.25f;

                            if (rand == 3)
                            {

                            }
                            if (rand == 4)
                            {

                            }
                            else
                            {
                                fish.AddComponent<FishBehavior>();
                                fish.GetComponent<FishBehavior>().player = player;
                                fish.GetComponent<FishBehavior>().type = rand + 1;
                                fish.GetComponent<FishBehavior>().generator = this;
                                fish.GetComponent<FishBehavior>().SetFish();
                                fish.GetComponent<FishBehavior>().enabled = true;
                            }

                            fishList.Add(fish);
                        }
                    }
                }
            }
        }
    }
}
