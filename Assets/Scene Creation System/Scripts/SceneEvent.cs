using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneEvent : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        public UnityEvent unityEvent;

        public bool debug = false;

        public bool Trigger()
        {
            if (!sceneConditions.VerifyConditions()) return false;

            sceneActions.Trigger();
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke();

            if (debug)
                DebugSceneEvent();

            return true;
        }

        
        public void Init()
        {
            sceneParameteredEvents.Init();
        }
        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneConditions.SetUp(_sceneVariablesSO);
            sceneActions.SetUp(_sceneVariablesSO);
            sceneParameteredEvents.SetUp(_sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneActions.BelongTo(_sceneObject);
            sceneParameteredEvents.BelongTo(_sceneObject);
        }

        private void DebugSceneEvent()
        {
            Debug.LogError("Triggered Scene Event : " + eventID);
        }
    }
}
