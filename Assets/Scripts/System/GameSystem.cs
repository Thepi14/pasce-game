using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public GameObject playerObject;
    public Image overlay;
    public void RestartGame()
    {
        playerObject.transform.position = Vector2.zero;
    }
    public void Start()
    {
        overlay.enabled = true;
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn()
    {
        overlay.gameObject.SetActive(true);
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            overlay.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.05f);
        }
        overlay.gameObject.SetActive(false);
    }
}
