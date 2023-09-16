using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhs5.SceneCreation;
using System.Text;

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
        listeners.SetEvents(Loog);
        dependencies.SetUp(sceneVariablesSO);
    }

    private void Loog(SceneEventParam param)
    {
        Debug.LogWarning(param);
    }

    protected override void Init()
    {
        base.Init();

        listeners.SetEvents(Loog);
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

    protected override void ChildLog(List<string> lines, StringBuilder sb, bool detailed)
    {
        base.ChildLog(lines, sb, detailed);

        foreach (var listener in listeners)
        {
            lines.AddRange(listener.LogLines(detailed));
        }
    }
}
