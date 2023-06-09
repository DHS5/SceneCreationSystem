using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dhs5.SceneCreation
{
    [DisallowMultipleComponent]
    public class SceneObject : MonoBehaviour
    {
        public SceneVariablesSO sceneVariablesSO;

        [Header("Listeners")]
        public List<SceneListener> sceneListeners;

        [Header("Actions")]
        public List<SceneEvent> sceneEvents;

        #region Scene Events subscription
        protected virtual void OnEnable()
        {
            if (sceneListeners != null)
            {
                foreach (SceneListener listener in sceneListeners)
                {
                    listener.Register();
                }
            }
        }
        protected virtual void OnDisable()
        {
            if (sceneListeners != null)
            {
                foreach (SceneListener listener in sceneListeners)
                {
                    listener.Unregister();
                }
            }
        }

        private List<SceneListener> GetListenersByID(int varUniqueID)
        {
            return sceneListeners.FindAll(l => l.UID == varUniqueID);
        }
        #endregion

        #region Update Listeners, Actions & Tweens
        private void Awake()
        {
            sceneEvents.Init();
            UpdateBelongings();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();
        }

        protected virtual void UpdateSceneVariables()
        {
            sceneListeners.SetUp(sceneVariablesSO);
            sceneEvents.SetUp(sceneVariablesSO);
        }
        protected virtual void UpdateBelongings()
        {
            sceneListeners.BelongTo(this);
            sceneEvents.BelongTo(this);
        }

        #endregion

        #region Trigger Action

        protected List<SceneEvent> GetSceneEventsByID(string eventID)
        {
            if (sceneEvents == null) return null;
            return sceneEvents.FindAll(a => a.eventID == eventID);
        }
        public void TriggerSceneEvent(string eventID)
        {
            sceneEvents.Trigger(eventID);
        }
        public void TriggerRandom(string filter)
        {
            sceneEvents.TriggerRandom(filter);
        }
        public void TriggerRandomAndRemove(string filter)
        {
            sceneEvents.TriggerRandom(filter, true);
        }
        #endregion
    }
}
