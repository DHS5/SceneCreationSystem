using Dhs5.SceneCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProfile : SceneProfile
{
    [SerializeField] private List<SceneEvent> specialEvents;

    public override bool CanOverrideListeners => false;

    public override bool CanOverrideEvents => false;

    protected override void RegisterSceneEventsLists()
    {
        
    }

    protected override void RegisterTweens()
    {
        
    }
}
