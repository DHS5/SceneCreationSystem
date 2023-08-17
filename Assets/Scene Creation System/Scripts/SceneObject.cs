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

        #region Trigger Events

        #region Exposed Functions
        public void TriggerSceneEvent(string eventID)
        {
            TriggerSceneEvent(eventID, default);
        }
        public void TriggerSceneEventAndRemove(string eventID)
        {
            TriggerSceneEventAndRemove(eventID, default, 1);
        }
        public void TriggerAllSceneEventAndRemove(int triggerNumber)
        {
            TriggerAllSceneEventAndRemove(default, triggerNumber);
        }
        [Preserve]
        public void TriggerSceneEventAndRemove(string eventID, int triggerNumber)
        {
            TriggerSceneEventAndRemove(eventID, default, triggerNumber);
        }
        public void TriggerRandom(string filter)
        {
            TriggerRandom(filter, default);
        }
        public void TriggerRandomAndRemove(string filter)
        {
            TriggerRandomAndRemove(filter, default);
        }
        #endregion

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

            TriggerEventInProfilesAndRemove(eventID, triggerNumber);
        }
        public void TriggerAllSceneEventAndRemove(SceneEventParam param = default, int triggerNumber = 1)
        {
            sceneEvents.TriggerAndRemoveAll(param, triggerNumber);

            TriggerAllEventsInProfilesAndRemove(triggerNumber);
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
            if (profiles == null || profiles.Count <= 0) return;

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
        [Preserve]
        public void TriggerEventInProfilesAndRemove(string eventID, int triggerNumber)
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return;

            foreach (var profile in sceneProfiles)
                profile.TriggerEventInProfileAndRemove(eventID, triggerNumber);
        }
        public void TriggerAllEventsInProfilesAndRemove(int triggerNumber)
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return;

            foreach (var profile in sceneProfiles)
                profile.TriggerAllEventsInProfileAndRemove(triggerNumber);
        }
        public void TriggerEventInProfilesAndRemove(string eventID)
        {
            TriggerEventInProfilesAndRemove(eventID, 1);
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
