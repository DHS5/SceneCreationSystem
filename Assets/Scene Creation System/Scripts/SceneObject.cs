using GluonGui.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Dhs5.SceneCreation
{
    [DisallowMultipleComponent]
    public class SceneObject : MonoBehaviour
    {
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;

        [Header("Listeners")]
        [SerializeField] protected List<SceneListener> sceneListeners;

        [Header("Actions")]
        [SerializeField] protected List<SceneEvent<SceneEventParam>> sceneEvents;

        #region Scene Events subscription
        private void OnEnable()
        {
            sceneListeners.Register();

            OnEnable_S();
        }
        private void OnDisable()
        {
            sceneListeners.Unregister();

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
            Init();
            UpdateBelongings();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();

            OnValidate_S();
        }

        protected virtual void Init()
        {
            sceneEvents.Init();
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

        protected List<SceneEvent<SceneEventParam>> GetSceneEventsByID(string eventID)
        {
            if (sceneEvents == null) return null;
            return sceneEvents.FindAll(a => a.eventID == eventID);
        }
        public void TriggerSceneEvent(string eventID)
        {
            sceneEvents.Trigger(eventID);
        
            TriggerEventInProfiles(eventID);
        }
        public void TriggerSceneEventAndRemove(string eventID)
        {
            sceneEvents.TriggerAndRemove(default, eventID, 1);
        }
        [Preserve]
        public void TriggerSceneEventAndRemove(string eventID, int triggerNumber)
        {
            sceneEvents.TriggerAndRemove(default, eventID, triggerNumber);
        }
        public void TriggerRandom(string filter)
        {
            sceneEvents.TriggerRandom(default, filter);
        }
        public void TriggerRandomAndRemove(string filter)
        {
            sceneEvents.TriggerRandom(default, filter, true);
        }
        public void TriggerSceneEvent(string eventID, SceneEventParam param = default)
        {
            sceneEvents.Trigger(param, eventID);

            TriggerEventInProfiles(eventID);
        }
        /// <summary>
        /// Trigger a <see cref="SceneEvent"/> <paramref name="triggerNumber"/> number of times then remove it from the list
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="param"></param>
        /// <param name="triggerNumber"></param>
        public void TriggerSceneEventAndRemove(string eventID, SceneEventParam param = default, int triggerNumber = 1)
        {
            sceneEvents.TriggerAndRemove(param, eventID, triggerNumber);
        }
        public void TriggerRandom(string filter, SceneEventParam param = default)
        {
            sceneEvents.TriggerRandom(param, filter);
        }
        public void TriggerRandomAndRemove(string filter, SceneEventParam param = default)
        {
            sceneEvents.TriggerRandom(param, filter, true);
        }
        #endregion

        #region Profiles
        protected List<SceneProfile> sceneProfiles = new();

        #region List Handling
        public void ApplyProfiles(List<SceneProfile> profiles)
        {
            ClearProfiles();

            foreach (SceneProfile profile in profiles)
            {
                sceneProfiles.Add(profile);
                profile.Attach(this);
            }
        }
        public void AddProfile(SceneProfile profile)
        {
            if (sceneProfiles == null) sceneProfiles = new();
            
            sceneProfiles.Add(profile);
        }
        public void RemoveProfile(SceneProfile profile)
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return;

            if (sceneProfiles.Contains(profile)) sceneProfiles.Remove(profile);
        }
        public bool RemoveProfileOfType<T>(bool all = false) where T : SceneProfile
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return false;

            bool result = false;

            foreach (SceneProfile profile in new List<SceneProfile>(sceneProfiles))
            {
                if (profile is T p)
                {
                    sceneProfiles.Remove(p);
                    if (!all) return true;
                    result = true;
                }
            }

            return result;
        }

        public void ClearProfiles()
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0)
            {
                sceneProfiles = new();
                return;
            }

            foreach (var profile in sceneProfiles)
            {
                profile.Detach();
            }
            sceneProfiles.Clear();
        }
        #endregion

        #region Actions
        public bool TriggerProfileOfType<T>(bool all = false) where T : SceneProfile
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return false;

            bool result = false;

            foreach (var profile in sceneProfiles)
                if (profile is T p)
                {
                    p.TriggerProfile();
                    if (!all) return true;
                    result = true;
                }
            return result;
        }
        public bool TriggerProfile(SceneProfile profile)
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return false;

            foreach (var p in sceneProfiles)
                if (p == profile)
                {
                    p.TriggerProfile();
                    return true;
                }
            return false;
        }
        public void TriggerAllProfiles()
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return;

            foreach (var profile in sceneProfiles)
                profile.TriggerProfile();
        }
        public void TriggerEventInProfiles(string eventID)
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return;

            foreach (var profile in sceneProfiles)
                profile.TriggerEventInProfile(eventID);
        }
        #endregion
        
        #region Random Actions
        public bool TriggerRandomInProfileOfType<T>(string filter = null) where T : SceneProfile
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return false;

            foreach (var profile in sceneProfiles)
                if (profile is T p)
                {
                    return p.TriggerProfileRandom(filter);
                }
            return false;
        }
        public bool TriggerRandomInProfileOfTypeAndRemove<T>(string filter = null) where T : SceneProfile
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return false;

            foreach (var profile in sceneProfiles)
                if (profile is T p)
                {
                    return p.TriggerProfileRandom(filter, true);
                }
            return false;
        }
        
        #endregion
        #endregion
    }
}
