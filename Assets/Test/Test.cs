using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dhs5.SceneCreation;
using TMPro;
using UnityEngine.Scripting;
using System;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public SceneSpawner spawner;
    public Color color;
    public SceneEvent sceneEvent;

    //[SerializeReference, SubclassPicker] BaseClass baseClass;
    //[SerializeReference, SubclassPicker] List<BaseClass> baseClasses = new();
    //
    //[Space(20f)]
    //public SelectList<SceneEvent> selectList;
    //[Space(20f)]
    //public List<Container> containers;
    [Space(20f)]
    public SceneTimelineList timelineList;

    [Serializable]
    public class Container
    {
        public SceneElementList<SceneCondition> conditions;
    }

    private void Awake()
    {
        //SceneManager.BalancingIndex = 1;
    }

    public void Test1(SceneEventParam param)
    {
        text1.text = param.Value + " " + param.FormerValue + " " + param.Sender;
    }

    [Preserve]
    public void Test2(int int1, float float1, string string1, bool bool1)
    {
        text2.text = string1 + int1 + " " + float1 + " " + bool1;
    }
    
    [Preserve]
    public void Test3()
    {
        text3.text = "yo did it";
    }

    public void ProfileTest()
    {
        BaseSceneObject sceneObject = spawner.SpawnAndRemove("Listening SceneObject", null);
        Debug.Log(sceneObject.TriggerProfileOfType<SceneEventProfile>());
        BaseSceneObject sceneObject2 = spawner.SpawnAndRemove("Listening SceneObject2", null);
        Debug.Log(sceneObject2.TriggerProfileOfType<SceneEventProfile>());
    }
}
