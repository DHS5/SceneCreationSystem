using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhs5.SceneCreation;

public class TestSceneObject : SceneObject
{
    public List<SceneSpecificListener> listeners;
    [Space(15f)]
    public List<SceneDependency> dependencies;

    public void Test(SceneEventParam param)
    {
        Debug.Log(param.ToString());
    }

    protected override void UpdateBelongings()
    {
        base.UpdateBelongings();

        listeners.BelongTo(this);
    }
    protected override void UpdateSceneVariables()
    {
        base.UpdateSceneVariables();

        listeners.SetUp(sceneVariablesSO);
        dependencies.SetUp(sceneVariablesSO);
    }
    protected override void Init()
    {
        base.Init();

        listeners.SetEvents((param) => { Debug.LogWarning(param); });
    }

    protected override void OnEnable_Ext()
    {
        base.OnEnable_Ext();

        listeners.Register();
    }

    protected override void OnDisable_Ext()
    {
        base.OnDisable_Ext();

        listeners.Unregister();
    }
}
