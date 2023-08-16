using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneListener : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        [Serializable]
        public struct SceneEventTrigger
        {
            public string eventID;
            public bool random;
            public bool remove;
        }

        public SceneVariablesSO sceneVariablesSO;
        private SceneObject sceneObject;

        // SceneVar selection
        [SerializeField] private int varUniqueID;
        public int UID => varUniqueID;
        public SceneVar CurrentSceneVar
        {
            get { return SceneState.GetSceneVar(varUniqueID); }
        }
        
        // Condition
        public bool hasCondition;

        public List<SceneCondition> conditions;
        
        public UnityEvent<SceneEventParam> events;

        public List<SceneEventTrigger> triggers;

        public bool debug = false;
        public float propertyHeight;

        #region Event Subscription
        public void Register()
        {
            SceneEventManager.StartListening(varUniqueID, OnListenerEvent);
        }
        public void Unregister()
        {
            SceneEventManager.StopListening(varUniqueID, OnListenerEvent);
        }
        private void OnListenerEvent(SceneEventParam param)
        {
            if (VerifyConditions())
            {
                events.Invoke(param);
                sceneObject.Trigger(triggers, param);
                if (debug)
                    Debug.LogError("Received event : " + CurrentSceneVar.ToString());
            }
        }
        #endregion


        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;

            conditions.SetUp(sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
        }

        public bool VerifyConditions()
        {
            return !hasCondition || conditions.VerifyConditions();
        }
    }
}
