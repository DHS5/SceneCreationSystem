using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhs5.SceneCreation;

public class ExampleSObj : SceneObject
{
    [Header("Example")]
    public SceneVarTween spawnFrequency;

    public SceneVarTween spawnQuantity;


    protected override void UpdateSceneVariables()
    {
        base.UpdateSceneVariables();

        spawnFrequency.SetUp(sceneVariablesSO, SceneVarType.FLOAT, true);

        spawnQuantity.SetUp(sceneVariablesSO, SceneVarType.INT, false);
    }
    protected override void SetBelongings()
    {
        base.SetBelongings();

        spawnFrequency.BelongTo(this);

        spawnQuantity.BelongTo(this);
    }
}


