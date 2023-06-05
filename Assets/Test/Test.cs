using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhs5.SceneCreation;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.BalancingIndex = 1;
    }

    public void Test1(SceneEventParam param)
    {
        Debug.Log(param.Value + " " + param.FormerValue + " " + param.Sender);
    }
}
