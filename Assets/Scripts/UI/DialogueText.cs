using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
public class DialogueText : ScriptableObject
{
    public string[] speakerName;
    public string[] paragraphs;
    public Sprite[] speakerImage;
}
