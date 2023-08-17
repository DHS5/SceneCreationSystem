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

        public int TriggerNumberLeft { get; private set; } = -1;
        bool triggerCount = false;

        public bool Trigger(int triggerNumber = -1)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }

            if (!sceneConditions.VerifyConditions()) return false;

            if (triggerCount) TriggerNumberLeft--;
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
    
    [Serializable]
    public class SceneEvent<T> : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        public UnityEvent<T> unityEvent;

        public bool debug = false;

        public int TriggerNumberLeft { get; private set; } = -1;
        bool triggerCount = false;

        public bool Trigger(int triggerNumber = -1)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }

            if (!sceneConditions.VerifyConditions()) return false;

            if (triggerCount) TriggerNumberLeft--;

            sceneActions.Trigger();
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke(default);

            if (debug)
                DebugSceneEvent();

            return true;
        }
        public bool Trigger(T param, int triggerNumber = -1)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }

            if (!sceneConditions.VerifyConditions()) return false;

            if (triggerCount) TriggerNumberLeft--;

            sceneActions.Trigger();
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke(param);

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
