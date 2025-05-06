using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class/New Tile")]
public class Tile : ScriptableObject
{
    [Header("Properties")]
    public string tileName;
    public Sprite image;
    public bool hasBorders = false;
    public Color color;
    public int order = 0;
}
