using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureFunction;
using static WallGraphicsGenerator;

public class WaveEffect : MonoBehaviour
{
    public int initialSize = 3;
    public int finalSize = 5;
    public float timer = 0.1f;
    private float _timer;
    public float counter;
    public SpriteRenderer ocean;
    private Color waveColor;
    private Rigidbody2D rb;
    public bool run = true;
    ~WaveEffect() => Console.WriteLine($"The momomo finalizer is executing.");
    void Start()
    {
        run = true;
        waveColor = ocean.color;
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        _timer = timer / ((((rb.velocity.magnitude * rb.velocity.normalized.magnitude) / 10) + 0.1f) * 10);
        if (run)
            counter += Time.deltaTime;
        if (counter >= _timer)
        {
            StartCoroutine(effect());
            counter = 0;
        }
    }
    IEnumerator effect()
    {
        Texture2D waveTexture;
        waveTexture = new(finalSize, finalSize);
        GameObject obj = new("Wave");
        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Water";
        obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
        obj.GetComponent<SpriteRenderer>().color = waveColor / 2 + Color.gray;
        obj.transform.position = gameObject.transform.position;
        obj.transform.parent = GAME.transform;
        float alper = (1 / ((float)finalSize - (float)initialSize));
        float alper2 = 0;
        for (int i = initialSize; i <= finalSize; i++)
        {
            for (int x = 0; x < waveTexture.width; x++)
            {
                for (int y = 0; y < waveTexture.height; y++)
                {
                    waveTexture.SetPixel(x, y, Color.clear);
                }
            }
            waveTexture.Apply();
            DrawCircle(ref waveTexture, new Color(0.9f, 0.9f, 0.9f, 1 - alper2), waveTexture.width / 2, waveTexture.height / 2, i / 2);
            DrawCircle(ref waveTexture, Color.clear, waveTexture.width / 2, waveTexture.height / 2, (i / 2) - 1);
            waveTexture.filterMode = FilterMode.Point;
            if (waveTexture.width % 2 == 0)
                waveTexture.Compress(false);
            waveTexture.Apply();
            Sprite finalSprite = Sprite.Create(waveTexture, new Rect(0, 0, waveTexture.width, waveTexture.height), new Vector2(0.5f, 0.5f), 16, 0, 0, new Vector4(0, 0, 0, 0), true);
            obj.GetComponent<SpriteRenderer>().sprite = finalSprite;
            alper2 += alper;
            yield return new WaitForSeconds(0.1f);
        }
        KillWave(obj);
    }
    private void KillWave(GameObject a)
    {
        Destroy(a);
    }
}
