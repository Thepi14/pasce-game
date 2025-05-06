using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Choices")]
public class DialogueChoice : ScriptableObject
{
    public string choiceText;
    public string[] choices;
}
