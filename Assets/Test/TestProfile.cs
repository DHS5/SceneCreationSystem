using Dhs5.SceneCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProfile : SceneProfile
{
    [SerializeField] private List<SceneEvent> specialEvents;

    protected override void RegisterSceneEventsLists()
    {
        
    }

    protected override void RegisterTweens()
    {
        
    }

    public override string Name => "Test Profile";

    public override bool Override<T>(T overridingProfile)
    {
        return false;
    }
}
