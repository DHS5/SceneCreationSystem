using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [DisallowMultipleComponent]
    public class SceneObject : MonoBehaviour
    {
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;

        [Header("Listeners")]
        [SerializeField] protected List<SceneListener> sceneListeners;

        [Header("Actions")]
        public List<SceneEvent> sceneEvents;

        #region Scene Events subscription
        private void OnEnable()
        {
            if (sceneListeners != null)
            {
                foreach (SceneListener listener in sceneListeners)
                {
                    listener.Register();
                }
            }

            OnEnable_S();
        }
        private void OnDisable()
        {
            if (sceneListeners != null)
            {
                foreach (SceneListener listener in sceneListeners)
                {
                    listener.Unregister();
                }
            }

            OnDisable_S();
        }

        protected virtual void OnEnable_S() { }
        protected virtual void OnDisable_S() { }

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

            OnValidate_S();
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

        protected virtual void OnValidate_S() { }

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
