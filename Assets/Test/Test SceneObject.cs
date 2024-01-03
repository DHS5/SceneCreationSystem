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
    [Space(15f)]
    public SceneVarTween _tween;

    public void Test(SceneEventParam param)
    {
        Debug.Log(param.ToString());
    }

    protected override void SetBelongings()
    {
        base.SetBelongings();

        listeners.BelongTo(this);
    }
    protected override void UpdateSceneVariables()
    {
        base.UpdateSceneVariables();

        listeners.SetUp(sceneVariablesSO);
        listeners.SetEvents(Loog);
        dependencies.SetUp(sceneVariablesSO);
        _tween.SetUp(SceneVariablesSO, SceneVarType.BOOL, false, false, true);
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

    protected override void OnSceneObjectEnable()
    {
        base.OnSceneObjectEnable();

        listeners.Subscribe();
    }

    protected override void OnSceneObjectDisable()
    {
        base.OnSceneObjectDisable();

        listeners.Unsubscribe();
    }

    public override void ChildLog(List<string> lines, StringBuilder sb, bool detailed, bool showEmpty, string alinea = null)
    {
        base.ChildLog(lines, sb, detailed, showEmpty);

        foreach (var listener in listeners)
        {
            lines.AddRange(listener.LogLines(detailed));
        }
    }
}
