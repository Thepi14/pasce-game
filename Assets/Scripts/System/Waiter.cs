using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Waiter : MonoBehaviour
{
    public void Update()
    {
        
    }
    
}
public class WaitConditionRef<T>
{
    public bool abortCondition { get; set; }
    private Coroutine coroutine;
    public WaitConditionRef(MonoBehaviour caller, Action onUpdate, T yieldReturn, Action onFinish)
    {
        coroutine = caller.StartCoroutine(WaitUntilRef(onUpdate, yieldReturn, onFinish));
    }
    private IEnumerator WaitUntilRef(Action onUpdate, T yieldReturn, Action onFinish)
    {
        while (!abortCondition)
        {
            onUpdate();
            yield return yieldReturn;
        }
        onFinish();
    }
}