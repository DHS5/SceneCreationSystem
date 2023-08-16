using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhs5.SceneCreation;

public class TestSceneObject : SceneObject
{
    public void Test(SceneEventParam param)
    {
        Debug.Log(param.ToString());
    }
}
