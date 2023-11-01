using Dhs5.SceneCreation;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestBaseSceneObject : BaseSceneObject
{
    [SerializeField] private List<SceneListener> listeners1;
    [SerializeField] private List<SceneSpecificListener> listeners2;
    [SerializeField] private List<SceneEvent> events;
    [Space]
    [SerializeField] private SceneObjectTag soTag;
    [SerializeField] private SceneObjectLayer soLayer;

    protected override void RegisterSceneElements()
    {
        RegisterListener(nameof(listeners1), listeners1);
        RegisterListener(nameof(listeners2), listeners2);
        RegisterEvent(nameof(events), events);
    }

    protected override void UpdateSceneVariables()
    {
        Setup(listeners1);
        Setup(listeners2);
        Setup(events);

        listeners2.SetEvents(Event);
    }

    private void Event(SceneEventParam param)
    {

    }
}
