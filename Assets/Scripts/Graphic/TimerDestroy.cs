using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerDestroy : MonoBehaviour
{
    private float timer;
    public SpriteRenderer Fout;
    public Color Cor;

    void Start()
    {
        Fout = GetComponent<SpriteRenderer>();
        Cor = Fout.color;
    }

    void Update()
    {
        timer += Time.deltaTime;
        Cor.a -= Time.deltaTime / 2f;
        Fout.color = Cor;
        
        if (timer > 1)
        {
            Destroy(gameObject, 0);
        }
        
    }
}
