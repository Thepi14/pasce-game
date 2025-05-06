using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhoIsEnablingThis : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("I just got enabled!", this);
    }
}
