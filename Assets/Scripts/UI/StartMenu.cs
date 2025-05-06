using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static TextureFunction;

public class StartMenu : MonoBehaviour
{
    [Header("Configurations", order = 1)]
    public float birdSpeed = 50;
    public float lampSpeed = 1;
    public float limit = 500;
    public Gradient skyGradientDay;
    public Gradient skyGradientNight;
    [Header("Attributes", order = 1)]
    public Image overlay;
    public Button play;
    public Button credits;
    public Button nuts;
    private float overCounter, lampPosCounterX, lampPosCounterY, buttonPosCounterY;
    public bool started, gaming, lampMode, firstRun, tutoring;
    public GameObject lamp, lampLight;
    public GameObject[] birds;
    public GameObject[] audios;
    public Image sky, trees;
    public Sprite sun, moon;
    public bool runChange = false, upButton = true, changingSky = false, passedCycle = false, crediting = false;
    private int L = 0;

    public Animator playAnim, outPlayAnim;
    public Animator creditsAnim, outCreditsAnim;

    void Start()
    {
        //lampLight = GameObject.Find("LampLight");
        play.onClick.AddListener(StartGame);
        credits.onClick.AddListener(StartCredits);
        nuts.onClick.AddListener(StartTutorial);

        play.onClick.AddListener(delegate { OnClickMenuButton("play"); });
        credits.onClick.AddListener(delegate { OnClickMenuButton("credits"); });

        playAnim = play.gameObject.GetComponent<Animator>();
        outPlayAnim = play.transform.Find("out").GetComponent<Animator>();
        creditsAnim = credits.gameObject.GetComponent<Animator>();
        outCreditsAnim = credits.transform.Find("out").GetComponent<Animator>();

        overCounter = 1f;
        started = true;
        gaming = false;
        lampMode = true;
        firstRun = true;
        lampPosCounterX = 0.5f;
        //StartCoroutine(Oscilate());
    }
    public void StartTutorial()
    {
        if (!started && !crediting && !tutoring)
            tutoring = true;
    }
    public void OnHoverMenuButton(string which)
    {
        if (which == "play")
        {
            playAnim.Play("Hover");
            outPlayAnim.Play("Hover");
        }
        else if (which == "credits")
        {
            creditsAnim.Play("Hover");
            outCreditsAnim.Play("Hover");
        }
    }
    public void OnClickMenuButton(string which)
    {
        if (which == "play")
        {
            playAnim.Play("Pressed");
            outPlayAnim.Play("Pressed");
        }
        else if (which == "credits")
        {
            creditsAnim.Play("Pressed");
            outCreditsAnim.Play("Pressed");
        }
    }
    public void OnExitMenuButton(string which)
    {
        if (which == "play")
        {
            playAnim.Play("Exit");
            outPlayAnim.Play("Exit");
        }
        else if (which == "credits")
        {
            creditsAnim.Play("Exit");
            outCreditsAnim.Play("Exit");
        }
    }
    void Update()
    {
        lampPosCounterX += Time.deltaTime * 25 * lampSpeed;

        if (lamp.GetComponent<RectTransform>().anchoredPosition.x < -160)
            lampPosCounterY += Time.deltaTime * 10 * lampSpeed;
        else if (lamp.GetComponent<RectTransform>().anchoredPosition.x < -70)
            lampPosCounterY += Time.deltaTime * 5 * lampSpeed;
        else if (lamp.GetComponent<RectTransform>().anchoredPosition.x > 160)
            lampPosCounterY -= Time.deltaTime * 10 * lampSpeed;
        else if (lamp.GetComponent<RectTransform>().anchoredPosition.x > 70)
            lampPosCounterY -= Time.deltaTime * 5 * lampSpeed;

        for (int b = 0; b < birds.Length; b++)
        {
            if (birds[b].GetComponent<RectTransform>().anchoredPosition.x > 440)
            {
                birds[b].GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-550, -470), Random.Range(-15, 15) + 84);
            }
            else
            {
                birds[b].GetComponent<RectTransform>().anchoredPosition += new Vector2(birdSpeed * Time.deltaTime, 0);
            }
            birds[b].GetComponent<Image>().color =  new Color(sky.color.r, sky.color.g, sky.color.b, birds[b].GetComponent<Image>().color.a);
        }

        if (buttonPosCounterY >= 10)
            upButton = false;
        else if (buttonPosCounterY <= -10)
            upButton = true;

        if (upButton)
            buttonPosCounterY += Time.deltaTime * 8;
        else
            buttonPosCounterY -= Time.deltaTime * 8;

        if (lampMode)
        {
            float a = (lampPosCounterX + limit) / (limit * 2);
            sky.color = skyGradientDay.Evaluate(a);
        }
        else
        {
            float a = (lampPosCounterX + limit) / (limit * 2);
            sky.color = skyGradientNight.Evaluate(a);
        }

        trees.color = sky.color;
        play.gameObject.GetComponent<Image>().color = sky.color;
        outPlayAnim.gameObject.GetComponent<Image>().color = sky.color;
        play.transform.Find("text").GetComponent<TextMeshProUGUI>().color = sky.color;

        credits.gameObject.GetComponent<Image>().color = sky.color;
        outCreditsAnim.gameObject.GetComponent<Image>().color = sky.color;
        credits.transform.Find("text").GetComponent<TextMeshProUGUI>().color = sky.color;

        play.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (play.gameObject.GetComponent<RectTransform>().anchoredPosition.x, buttonPosCounterY - 23, 0);
        credits.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (credits.gameObject.GetComponent<RectTransform>().anchoredPosition.x, -buttonPosCounterY - 149, 0);
        play.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, buttonPosCounterY * 0.05f);
        credits.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -buttonPosCounterY * 0.05f);


        if (lampPosCounterX >= limit || firstRun)
        {
            if (!firstRun && lampPosCounterX >= limit)
            {
                lampPosCounterX = -limit;
                lampMode = !lampMode;
            }
            if (lampMode)
            {
                lamp.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
                //lampLight.GetComponent<RectTransform>().localScale = Vector3.one;
                lamp.GetComponent<Image>().sprite = sun;
                lampLight.GetComponent<Image>().sprite = sun;
                lampLight.GetComponent<Image>().sprite.name = "Sun";
                Sprite sprite;
                if (lampMode)
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(0, 0, 60, 60), new Vector2(0.5f, 0.5f), 1);
                else
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(60, 0, 60, 60), new Vector2(0.5f, 0.5f), 1);
                lampLight.GetComponent<Image>().sprite = sprite;

                StartCoroutine(FadeOutObjInCanvas(2f, false));
            }
            else
            {
                lamp.GetComponent<RectTransform>().localScale = Vector3.one;
                //lampLight.GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 1);
                lamp.GetComponent<Image>().sprite = moon;
                lampLight.GetComponent<Image>().sprite = moon;
                lampLight.GetComponent<Image>().sprite.name = "Moon";
                Sprite sprite;
                if (lampMode)
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(0, 0, 60, 60), new Vector2(0.5f, 0.5f), 10);
                else
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(60, 0, 60, 60), new Vector2(0.5f, 0.5f), 10);
                lampLight.GetComponent<Image>().sprite = sprite;

                StartCoroutine(FadeOutObjInCanvas(2f, true));
            }
            firstRun = false;
        }
        runChange = false;
        lamp.GetComponent<RectTransform>().anchoredPosition = new Vector3(lampPosCounterX, lampPosCounterY + 116, 0);

        if (overCounter <= 0)
        {
            overCounter = 0;
            started = false;
            overlay.color = new Color(0, 0, 0, 0);
        }
        if (started)
        {
            overCounter -= Time.deltaTime * 0.5f;
            overlay.color = new Color(1, 1, 1, overCounter);
        }
        if (gaming || crediting || tutoring)
        {
            overCounter += Time.deltaTime * 0.5f;
            overlay.color = new Color(0, 0, 0, overCounter);
            //play.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1 - overCounter, 1 - overCounter, 0);
            //credits.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1 - overCounter, 1 - overCounter, 0);
            for (int i = 0; i < audios.Length; i++)
            {
                audios[i].GetComponent<AudioSource>().volume = 1 - overCounter;
            }
        }
        if (overCounter >= 1 && gaming)
            SceneManager.LoadScene(1);
        else if (overCounter >= 1 && crediting)
            SceneManager.LoadScene(2);
        else if(overCounter >= 1 && tutoring)
            SceneManager.LoadScene(3);
    }
    IEnumerator Oscilate()
    {
        while (true)
        {
            L++;
            print(L);
            for (int s = 1; s <= 4; s++)
            {
                Sprite sprite;
                if (lampMode)
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(0, 0, 60, 60), new Vector2(0.5f, 0.5f), s);
                else
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(60, 0, 60, 60), new Vector2(0.5f, 0.5f), s);

                lampLight.GetComponent<Image>().sprite = sprite;
                yield return new WaitForSeconds(0.9f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int s = 4; s >= 1; s--)
            {
                Sprite sprite;
                if (lampMode)
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(0, 0, 60, 60), new Vector2(0.5f, 0.5f), s);
                else
                    sprite = Sprite.Create(ApplyBlurAlpha(lamp.GetComponent<Image>().sprite.texture, 5), new Rect(60, 0, 60, 60), new Vector2(0.5f, 0.5f), s);

                lampLight.GetComponent<Image>().sprite = sprite;
                yield return new WaitForSeconds(0.9f);
            }
            if (gaming)
                break;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void StartGame()
    {
        if (!started && !crediting && !tutoring)
            gaming = true;
    }
    public void StartCredits()
    {
        if (!started && !crediting && !tutoring)
            crediting = true;
    }
    IEnumerator FadeOutObjInCanvas(float time, bool up)
    {
        var cor = birds[0].GetComponent<Image>().color;
        if (up)
        {
            for (float i = 1; i > 0; i -= 0.1f / time)
            {
                for (int b = 0; b < birds.Length; b++)
                {
                    birds[b].GetComponent<Image>().color = new Color(cor.r, cor.g, cor.b, i);
                }
                yield return new WaitForSeconds(0.1f);
            }
            for (int b = 0; b < birds.Length; b++)
            {
                birds[b].GetComponent<Image>().color = new Color(cor.r, cor.g, cor.b, 0);
            }
        }
        else
        {
            for (float i = 0; i < 1; i += 0.1f / time)
            {
                for (int b = 0; b < birds.Length; b++)
                {
                    birds[b].GetComponent<Image>().color = new Color(cor.r, cor.g, cor.b, i);
                }
                yield return new WaitForSeconds(0.1f);
            }
            for (int b = 0; b < birds.Length; b++)
            {
                birds[b].GetComponent<Image>().color = new Color(cor.r, cor.g, cor.b, 1);
            }
        }
    }
}
