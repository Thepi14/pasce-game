using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureFunction;
using static WallGraphicsGenerator;

public class OceanAudioEffect : MonoBehaviour
{
    public AudioSource oceanAudio;
    public Texture2D oceanAudioSource;

    private void Start()
    {
        oceanAudio = GetComponent<AudioSource>();
        oceanAudioSource = GetTexture(publicWhiteMap);
        for (int i = 0; i < 3; i++)
            oceanAudioSource = ExpandBlackArea(oceanAudioSource);
        oceanAudioSource = ApplyBlur(oceanAudioSource, 3);
    }
    void Awake()
    {
    }
    void Update()
    {
        oceanAudio.volume = 1f - oceanAudioSource.GetPixel((int)transform.position.x, (int)transform.position.y).r;
    }
}
