using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Creditis : MonoBehaviour
{
    public GameObject title;
    public GameObject creditsText;
    public float speed = 5f;

    private float fadeTitleCount = 0;

    private bool startScroll = false, fadeTitle = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        if (!fadeTitle)
        {
            fadeTitleCount += Time.deltaTime * 0.2f;
        }
        else if (fadeTitle && !startScroll)
        {
            fadeTitleCount -= Time.deltaTime * 0.3f;
        }
        if (fadeTitleCount >= 1)
        {
            fadeTitleCount = 1;
            fadeTitle = true;
        }
        else if (fadeTitle && fadeTitleCount <= 0)
        {
            fadeTitleCount = 0;
            startScroll = true;
        }
        title.GetComponent<Image>().color = new Color(1, 1, 1, fadeTitleCount);
        if (startScroll)
        {
            creditsText.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0, creditsText.GetComponent<RectTransform>().anchoredPosition.y + (Time.deltaTime * speed));
        }
    }
}
