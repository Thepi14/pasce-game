using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Button butt;
    void Start()
    {
        butt.onClick.AddListener(a);
    }
    void Update()
    {
        
    }
    public void a()
    {
        SceneManager.LoadScene(0);
    }
}
