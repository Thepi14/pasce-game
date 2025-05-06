using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static WallGraphicsGenerator;
using static TextureFunction;

public class Player : MonoBehaviour
{
    [Header("PLayer Stats")]
    public float maxHitPoints = 100;
    public float hitPoints = 100;
    public float speed;
    public float maxChargeForce = 2.5f;
    public float harpRechargeTime = 0.7f;
    [Header("System Parameters")]
    public float timer;
    public float footSpawnX;
    public float footSpawnY;
    private float horizontal;
    private float vertical;
    private float hor;
    private float ver;
    private float counter;
    private float val;
    [Header("Essential Things")]
    public PlayerUI playerUI;
    public CinemachineLol cameraLol;
    public Sprite boatSpriteUp, boatSpriteDown, boatSpriteSides;
    public Animator anim, boatAnim;
    public Efeitos FX;
    public Sprite footSprite;
    private Rigidbody2D rb, boatRb;
    private Vector2 PDir;
    public GameObject boat;
    public GameObject waitr;
    public GameObject playerSub;
    [Header("Harpoon")]
    public GameObject harpoon;
    public AudioSource harpoonSoundSource;
    public AudioClip[] harpoonLaunchSounds;
    private Harpoon harpScript;
    public GameObject rope;
    [Header("Others")]
    public MonoBehaviour waiter;
    public Text textInBoat, textOutBoat;
    public GameObject mapGenerator;
    public Texture2D mapTerrain;
    public AudioClip[] stepSFXs;
    public bool onBoat { get; private set; }
    private bool runningMessage = false;
    private bool canExit = false;

    public bool onUI = false;
    public bool onNPCUI = false;

    public float harpCounter, rechargeCounter;
    private GameObject wavePoint;
    private WaveEffect wavePointEffect;
    public bool clickedAndHolding, harpClicked, harpLaunched;
    private bool gameStarted;

    void Start()
    {
        playerUI.playerScript = this;
        gameStarted = true;
        harpoon.transform.position = transform.position;
        wavePoint = transform.Find("WavePoint").gameObject;
        wavePointEffect = wavePoint.GetComponent<WaveEffect>();
        rb = GetComponent<Rigidbody2D>();
        onBoat = false;
        boatAnim = boat.GetComponent<Animator>();
        FX = new Efeitos();
        boatRb = boat.GetComponent<Rigidbody2D>();
        speed = 5;
        FX.ShipOnSeaEffect(this, boat, 0.2f, 0.7f);
        harpScript = harpoon.GetComponent<Harpoon>();
        //FX.WavesEffect(this, boat, 8, 20, 5);
        LoadChunks();
    }
    private void OnValidate()
    {
        mapTerrain = mapGenerator.GetComponent<WallGraphicsGenerator>().map;
        //footSprite = (Sprite)Resources.Load("Player/pegada");
    }
    void Update()
    {
        playerUI.hpBar.maxValue = maxHitPoints;
        playerUI.hpBar.value = hitPoints;
        if (rb.velocity != Vector2.zero || boatRb.velocity != Vector2.zero)
            LoadChunks();
        wavePointEffect.run = !onBoat;
        wavePoint.transform.position = transform.position + new Vector3(GetComponent<CapsuleCollider2D>().offset.x * -horizontal, GetComponent<CapsuleCollider2D>().offset.y, 0);
        Texture2D tex;
        tex = GenerateHeightWhiteTex(new Texture2D(150, 200), 200 - (int)(publicGradientMap.GetPixel((int)transform.position.x, (int)transform.position.y).r * 200));
        if (publicWhiteMap.GetPixel((int)transform.position.x, (int)transform.position.y) == Color.black)
        {
            playerSub.GetComponent<SpriteMask>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 110);
        }
        //print(200 - (int)(publicGradientMap.GetPixel((int)transform.position.x, (int)transform.position.y).r * 200));

        if (!harpoon.GetComponent<SpriteRenderer>().enabled)
        {
            harpScript.shoot = false;
            harpScript.returning = false;
            harpCounter = 0;
            harpClicked = false;
            harpLaunched = false;
            clickedAndHolding = false;
        }
        rope.GetComponent<SpriteRenderer>().enabled = harpoon.GetComponent<SpriteRenderer>().enabled && Vector2.Distance(transform.position, harpoon.transform.position) > 0.7f;
        rope.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        //UnityEngine.Random.InitState(Random.Range(10000, 50000));
        GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt((transform.position.y - 0.5f) * 10);
        boat.GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt((boat.transform.position.y - 0.5f) * 10);
        speed = 5;

        boatAnim.enabled = onBoat;
        if (!onUI && !onNPCUI)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            hor = horizontal != 0 ? horizontal : vertical == 0 ? hor : 0;
            ver = vertical != 0 ? vertical : horizontal == 0 ? ver : 0;
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Vector2.Distance(transform.position, boat.transform.position) < 4 && onBoat == false)
                {
                    onBoat = true;
                    transform.position = boat.transform.position;
                    transform.parent = boat.transform;
                    cameraLol.target = boat;
                    EnablePlayer(false);
                }
                else if (onBoat == true)
                {
                    var first = true;
                    Vector3 returnToTerrain = transform.position + new Vector3((int)transform.position.x + 9000, (int)transform.position.y + 9000, transform.position.z);
                    for (int x = -3; x < 3; x++)
                    {
                        for (int y = -3; y < 3; y++)
                        {
                            if ((int)transform.position.x + x > 0 && (int)transform.position.y + y > 0 && 
                                (int)transform.position.x + x < publicWhiteMap.width && (int)transform.position.y + y < publicWhiteMap.height && 
                                publicWhiteMap.GetPixel((int)transform.position.x + x, (int)transform.position.y + y) == Color.white && 
                                (Vector2.Distance(transform.position, new Vector2((int)transform.position.x + x + 0.5f, (int)transform.position.y + y + 0.5f)) <= Vector2.Distance(transform.position, returnToTerrain) || first))
                            {
                                /*Debug.Log("Is white: " + (publicWhiteMap.GetPixel((int)transform.position.x + x, (int)transform.position.y + y) == Color.white).ToString());
                                Debug.Log("Is less: " + (Vector2.Distance(transform.position, new Vector2((int)transform.position.x + x + 0.5f, (int)transform.position.y + y + 0.5f)) <= Vector2.Distance(transform.position, returnToTerrain)).ToString());
                                Debug.Log("First: " + first);*/

                                first = false;
                                returnToTerrain = new Vector3((int)transform.position.x + x + 0.5f, (int)transform.position.y + y + 0.5f, transform.position.z);
                                canExit = true;
                            }
                        }
                    }
                    if (canExit == true)
                    {
                        transform.parent = GAME.transform;
                        onBoat = false;
                        boatAnim.enabled = onBoat;
                        cameraLol.target = gameObject;
                        if (hor != 0)
                        {
                            boat.GetComponent<SpriteRenderer>().sprite = boatSpriteSides;
                        }
                        else if (ver > 0)
                        {
                            boat.GetComponent<SpriteRenderer>().sprite = boatSpriteUp;
                        }
                        else if (ver < 0)
                        {
                            boat.GetComponent<SpriteRenderer>().sprite = boatSpriteDown;
                        }
                        transform.position = returnToTerrain;
                        EnablePlayer(true);
                    }
                    if (canExit == false && !runningMessage)
                    {
                        WaitConditionRef<Color> FadeInAndOutText = new(waiter, () => { }, textOutBoat.color, () => { });
                        FadeInAndOutText = new WaitConditionRef<Color>(waiter,
                            () =>
                            {
                                runningMessage = true;
                                FadeInAndOutText.abortCondition = true;
                                StartCoroutine(fade());
                                IEnumerator fade()
                                {
                                    float time = 4;
                                    for (float timer = 0; timer <= time; timer += 0.05f)
                                    {
                                        if (timer <= 1)
                                            textOutBoat.color = new Color(textOutBoat.color.r, textOutBoat.color.g, textOutBoat.color.b, timer);
                                        yield return new WaitForSeconds(0.05f);
                                    }
                                    for (float timer = time / 2; timer >= 0; timer -= 0.05f)
                                    {
                                        if (timer >= 0)
                                            textOutBoat.color = new Color(textOutBoat.color.r, textOutBoat.color.g, textOutBoat.color.b, timer);
                                        yield return new WaitForSeconds(0.05f);
                                    }
                                    runningMessage = false;
                                }
                            },
                            textOutBoat.color,
                            () => { textOutBoat.color = new Color(textOutBoat.color.r, textOutBoat.color.g, textOutBoat.color.b, 0); }
                        );
                    }
                }
                else if (!runningMessage)
                {
                    WaitConditionRef<Color> FadeInAndOutText = new(waiter, () => { }, textInBoat.color, () => { });
                    FadeInAndOutText = new WaitConditionRef<Color>(waiter,
                        () =>
                        {
                            runningMessage = true;
                            FadeInAndOutText.abortCondition = true;
                            StartCoroutine(fade());
                            IEnumerator fade()
                            {
                                float time = 4;
                                for (float timer = 0; timer <= time; timer += 0.05f)
                                {
                                    if (timer <= 1)
                                        textInBoat.color = new Color(textInBoat.color.r, textInBoat.color.g, textInBoat.color.b, timer);
                                    yield return new WaitForSeconds(0.05f);
                                }
                                for (float timer = time / 2; timer >= 0; timer -= 0.05f)
                                {
                                    if (timer >= 0)
                                        textInBoat.color = new Color(textInBoat.color.r, textInBoat.color.g, textInBoat.color.b, timer);
                                    yield return new WaitForSeconds(0.05f);
                                }
                                runningMessage = false;
                            }
                        },
                        textInBoat.color,
                        () => { textInBoat.color = new Color(textInBoat.color.r, textInBoat.color.g, textInBoat.color.b, 0); }
                    );
                }
            }
            if (Input.GetKey(KeyCode.Semicolon) && onBoat == false)
            {
                anim.Play("PlayerTinyEasterEggAnim");
                speed = 0;
            }
            else if (horizontal != 0 && (rb.velocity.x != 0 || boatRb.velocity.x != 0))
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyRunSidesAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyRunSidesAnimSub");
                }
                else
                    boatAnim.Play("ShipSides");
            }
            else if (vertical > 0 && (rb.velocity.y > 0 || boatRb.velocity.y > 0))
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyRunBackAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyRunBackAnimSub");
                }
                else
                    boatAnim.Play("ShipUp");
            }
            else if (vertical < 0 && (rb.velocity.y < 0 || boatRb.velocity.y < 0))
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyRunFrontAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyRunFrontAnimSub");
                }
                else
                    boatAnim.Play("ShipDown");
            }
            else if (horizontal == 0 && hor != 0)
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleSidesAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleSidesAnimSub");
                }
            }
            else if (vertical == 0 && ver > 0)
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleBackAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleBackAnimSub");
                }
            }
            else if (vertical == 0 && ver < 0)
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleFrontAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleFrontAnimSub");
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!harpLaunched && harpScript.returning == false)
                    harpClicked = true;
            }
            if (Input.GetMouseButton(0))
            {
                if (harpScript.returning == false && !harpLaunched)
                {
                    float v = Mathf.Atan2(Input.mousePosition.y - Camera.main.WorldToScreenPoint(transform.position).y, Input.mousePosition.x - Camera.main.WorldToScreenPoint(transform.position).x);
                    harpoon.transform.eulerAngles = new Vector3(0, 0, (Mathf.Rad2Deg * v) - 90);
                    harpoon.transform.position = transform.position - ((harpoon.transform.up * harpCounter) * 0.05f);
                    activateArpoon(true);
                    if (rechargeCounter >= harpRechargeTime)
                    {
                        harpCounter += Time.deltaTime;
                        if (harpCounter > 0.75f && harpCounter < maxChargeForce)
                        {
                            clickedAndHolding = true;
                        }
                        if (harpCounter >= maxChargeForce)
                        {
                            harpCounter = maxChargeForce;
                        }
                        else
                        {

                        }
                    }
                }
            }
            else if (harpLaunched)
            {

            }
            if (Input.GetMouseButtonUp(0))
            {
                rechargeCounter = 0;
                if (!harpLaunched && harpScript.returning == false)
                {
                    if (clickedAndHolding == false)
                    {
                        harpoonSoundSource.clip = harpoonLaunchSounds[0];
                        harpoonSoundSource.Play();
                    }
                    else
                    {
                        harpoonSoundSource.clip = harpoonLaunchSounds[1];
                        harpoonSoundSource.Play();
                    }
                    harpClicked = false;
                    clickedAndHolding = false;
                    harpScript.shoot = true;
                    harpLaunched = true;
                    //harpCounter = 0;
                }
            }

            if (Input.GetMouseButtonDown(1) && harpScript.returning == false && harpScript.shoot == true)
            {
                harpScript.shoot = false;
                harpScript.returning = true;
            }
            if (onBoat == false)
                Flip();
            else
                Flip(boat);

            counter += Time.deltaTime;

            //areia
            if (onBoat == false && counter > timer && (vertical != 0 || horizontal != 0) && mapTerrain.GetPixel((int)transform.position.x, (int)transform.position.y) == mapGenerator.GetComponent<WallGraphicsGenerator>().colorsTex.GetPixel(0, 0))
            {
                footSpawnX = -footSpawnX;
                counter = 0;
                FX.StepFX(footSprite, 0.5f, rb.velocity, transform, rb, footSpawnY, footSpawnX - 0.1f);
                //Destroy(2.0f);

                wavePoint.GetComponent<AudioSource>().clip = stepSFXs[0];
                wavePoint.GetComponent<AudioSource>().panStereo = -0.48f;
                wavePoint.GetComponent<AudioSource>().Play();
            }
            //grama
            else if (onBoat == false && counter > timer && (vertical != 0 || horizontal != 0) && mapTerrain.GetPixel((int)transform.position.x, (int)transform.position.y) == mapGenerator.GetComponent<WallGraphicsGenerator>().colorsTex.GetPixel(1, 0))
            {
                counter = 0;
                wavePoint.GetComponent<AudioSource>().clip = stepSFXs[1];
                wavePoint.GetComponent<AudioSource>().panStereo = -0.48f;
                wavePoint.GetComponent<AudioSource>().Play();
            }
            //terra
            else if (onBoat == false && counter > timer && (vertical != 0 || horizontal != 0) && mapTerrain.GetPixel((int)transform.position.x, (int)transform.position.y) == mapGenerator.GetComponent<WallGraphicsGenerator>().colorsTex.GetPixel(2, 0))
            {
                counter = 0;
                wavePoint.GetComponent<AudioSource>().clip = stepSFXs[2];
                wavePoint.GetComponent<AudioSource>().panStereo = -0.48f;
                wavePoint.GetComponent<AudioSource>().Play();
            }
            //água
            else if (onBoat == false && counter > timer && (vertical != 0 || horizontal != 0) && publicWhiteMap.GetPixel((int)transform.position.x, (int)transform.position.y) == Color.black)
            {
                counter = 0;
                wavePoint.GetComponent<AudioSource>().clip = stepSFXs[stepSFXs.Length - 1];
                wavePoint.GetComponent<AudioSource>().panStereo = 0f;
                wavePoint.GetComponent<AudioSource>().Play();
            }
        }
        else if (onUI || onNPCUI)
        {
            if (horizontal == 0 && hor != 0)
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleSidesAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleSidesAnimSub");
                }
            }
            else if (vertical == 0 && ver > 0)
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleBackAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleBackAnimSub");
                }
            }
            else if (vertical == 0 && ver < 0)
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleFrontAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleFrontAnimSub");
                }
            }
            else
            {
                if (onBoat == false)
                {
                    anim.Play("PlayerTinyIdleFrontAnim");
                    playerSub.GetComponent<Animator>().Play("PlayerTinyIdleFrontAnimSub");
                }
            }
        }
        if (harpClicked == false && clickedAndHolding == false && harpScript.shoot == false && harpScript.returning == false)
        {
            clickedAndHolding = false;
            harpoon.transform.position = transform.position;
            harpCounter = 0;
            activateArpoon(false);
        }
        if (!harpLaunched && !harpScript.returning)
        {
            playerUI.charge = harpCounter / maxChargeForce;
            rechargeCounter += Time.deltaTime;
            if (rechargeCounter > harpRechargeTime)
                rechargeCounter = harpRechargeTime;
        }
        else
            playerUI.charge = 0;
    }
    public void ReturnHarpoon()
    {
        harpScript.shoot = false;
        harpLaunched = false;
        harpScript.returning = true;
        //harpoon.SetActive(false);
    }
    public void EnablePlayer(bool enable)
    {
        rb.simulated = enable;
        GetComponent<CapsuleCollider2D>().enabled = enable;
        GetComponent<Animator>().enabled = enable;
        GetComponent<SpriteRenderer>().enabled = enable;
        playerSub.GetComponent<SpriteRenderer>().enabled = enable;
    }
    public void activateArpoon(bool enable)
    {
        //harpoon.GetComponent<Rigidbody2D>().simulated = enable;
        harpoon.GetComponent<CapsuleCollider2D>().enabled = enable;
        harpoon.GetComponent<SpriteRenderer>().enabled = enable;
    }
    public void GoToFrame(int value)
    {
        anim.Play("PlayerTinyEasterEggAnim", 0, (1f / 42f) * value);
    }
    public void ShuffleAnimation()
    {

        if (Random.Range(0, 10) > 5)
        {
            anim.Play("PlayerTinyEasterEggAnim", 0, (1f / 42f) * 8);
        }
        else
        {
            anim.Play("PlayerTinyEasterEggAnim", 0, (1f / 42f) * 23);
        }
    }

    private void Flip()
    {
        if (horizontal == 1) { transform.rotation = Quaternion.Euler(0, 0, 0); }
        else if (horizontal == -1) { transform.rotation = Quaternion.Euler(0, 180, 0); }
    }
    private void Flip(GameObject obj)
    {
        if (horizontal == 1) { obj.transform.rotation = Quaternion.Euler(0, 0, 0); }
        else if (horizontal == -1) { obj.transform.rotation = Quaternion.Euler(0, 180, 0); }
    }
    public void LoadChunks()
    {
        for (int i = 0; i < mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks.Count; i++)
        {
            if (mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].transform.position.y <= transform.position.y)
            {
                if (Vector2.Distance(transform.position, mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].transform.position - new Vector3(0, 20)) > 60)
                {
                    mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].SetActive(false);
                }
                else
                {
                    mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].SetActive(true);
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].transform.position + new Vector3(0, 20)) > 60)
                {
                    mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].SetActive(false);
                }
                else
                {
                    mapGenerator.GetComponent<WallGraphicsGenerator>().worldChunks[i].SetActive(true);
                }
            }
        }
    }
    void FixedUpdate()
    {
        //playerSub.transform.position = transform.position;
        playerSub.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 4;
        if (!onUI && !onNPCUI)
        {
            PDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
        else
        {
            PDir = Vector2.zero;
        }
        if (onBoat == false)
        {
            rb.velocity = PDir * speed;
            //boatRb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (boatRb.velocity.x < speed * 2 && boatRb.velocity.x > -speed * 2)
                boatRb.AddForce(new Vector2(PDir.x, 0));
            if (boatRb.velocity.y < speed * 2 && boatRb.velocity.y > -speed * 2)
                boatRb.AddForce(new Vector2(0, PDir.y));
        }
        /*if (boatRb.velocity.x > speed * 2 || boatRb.velocity.x < -speed * 2)
            boatRb.velocity = new Vector2(PDir.x * speed * 2, boatRb.velocity.y);
        if (boatRb.velocity.y > speed * 2 || boatRb.velocity.y < -speed * 2)
            boatRb.velocity = new Vector2(boatRb.velocity.x, PDir.y * speed * 2);*/

        if (boatRb.velocity.x > 0 && PDir.x == 0)
            boatRb.velocity = new Vector2(boatRb.velocity.x - 0.01f, boatRb.velocity.y);
        else if (boatRb.velocity.x < 0 && PDir.x >= 0)
            boatRb.velocity = new Vector2(boatRb.velocity.x + 0.01f, boatRb.velocity.y);
        if (boatRb.velocity.y > 0 && PDir.y == 0)
            boatRb.velocity = new Vector2(boatRb.velocity.x, boatRb.velocity.y - 0.01f);
        else if (boatRb.velocity.y < 0 && PDir.y == 0)
            boatRb.velocity = new Vector2(boatRb.velocity.x, boatRb.velocity.y + 0.01f);
        if (PDir.x == 0 && PDir.y == 0 && boatRb.velocity.y <= 0.02f && boatRb.velocity.y >= -0.02f && boatRb.velocity.x <= 0.02f && boatRb.velocity.x >= -0.02f)
            boatRb.velocity = Vector2.zero;

        wavePoint.GetComponent<Rigidbody2D>().velocity = rb.velocity;
        if (Vector2.Distance(transform.position, harpoon.transform.position) >= 20)
        {
            harpScript.shoot = false;
            harpScript.returning = true;
        }
        //rb.MovePosition(rb.position + PDir * speed * Time.fixedDeltaTime);
    }
}