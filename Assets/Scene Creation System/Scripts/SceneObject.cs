using GluonGui.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System.Text;

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

        #region Scene Listeners registration
        private void OnEnable()
        {
            sceneListeners.Register();

            OnEnable_Ext();
        }
        private void OnDisable()
        {
            sceneListeners.Unregister();

            OnDisable_Ext();
        }

        protected virtual void OnEnable_Ext() { }
        protected virtual void OnDisable_Ext() { }
        #endregion

        #region Update Listeners, Actions & Tweens
        private void Awake()
        {
            Init();
            UpdateBelongings();

            Awake_Ext();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();

            OnValidate_Ext();
        }
        protected virtual void Init()
        {
            sceneEventsList = new();

            sceneEvents.Init();

            RegisterElements();
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

        protected virtual void Awake_Ext() { }
        protected virtual void OnValidate_Ext() { }
        #endregion

        #region Scene Events Registration
        protected List<List<SceneEvent>> sceneEventsList;
        protected List<SceneVarTween> tweenList;
        /// <summary>
        /// Function where all <see cref="SceneEvent"/> and <see cref="SceneVarTween"/> lists should be registered with : <see cref="Register"/>
        /// </summary>
        protected virtual void RegisterElements() { }

        protected void Register(List<SceneEvent> events)
        {
            sceneEventsList.Add(events);
            events.Init();
            events.BelongTo(this);
        }
        protected void Register(SceneVarTween tween)
        {
            tweenList.Add(tween);
            tween.BelongTo(this);
        }
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

        #region SceneLog
        [ContextMenu("Display Log")]
        private void DisplayLog()
        {
            Debug.Log(Log(true));
        }

        public string Log(bool detailed = false)
        {
            StringBuilder sb = new();

            foreach (var l in LogLines(detailed))
            {
                sb.Append(l);
            }

            return sb.ToString();
        }
        public List<string> LogLines(bool detailed = false)
        {
            List<string> lines = new();
            StringBuilder sb = new();

            sb.Append(SceneLogger.SceneObjectColor);
            sb.Append("--------------------------------------------------");
            sb.Append(SceneLogger.ColorEnd);
            Line();

            sb.Append(SceneLogger.ListenerColor);
            sb.Append("Listeners :");
            sb.Append(SceneLogger.ColorEnd);
            Line();

            if (detailed)
            {
                sb.Append(SceneLogger.ListenerColor);
                sb.Append("----------------------------------------");
                sb.Append(SceneLogger.ColorEnd);
                Line();
            }

            foreach (var listener in sceneListeners)
            {
                lines.AddRange(listener.LogLines(detailed));
            }

            if (detailed)
            {
                sb.Append(SceneLogger.ListenerColor);
                sb.Append("----------------------------------------");
                sb.Append(SceneLogger.ColorEnd);
                Line();
            }

            sb.Append(SceneLogger.EventColor);
            sb.Append("Events :");
            sb.Append(SceneLogger.ColorEnd);
            Line();

            if (detailed)
            {
                sb.Append(SceneLogger.EventColor);
                sb.Append("----------------------------------------");
                sb.Append(SceneLogger.ColorEnd);
                Line();
            }

            foreach (var events in sceneEvents)
            {
                lines.AddRange(events.LogLines(detailed));
            }

            if (detailed)
            {
                sb.Append(SceneLogger.EventColor);
                sb.Append("----------------------------------------");
                sb.Append(SceneLogger.ColorEnd);
                Line();
            }

            sb.Append(SceneLogger.SceneObjectColor);
            sb.Append("--------------------------------------------------");
            sb.Append(SceneLogger.ColorEnd);
            lines.Add(sb.ToString());
            //Line();

            return lines;

            #region Local
            void Line()
            {
                sb.Append('\n');
                lines.Add(sb.ToString());
                sb.Clear();
            }
            #endregion
        }
        #endregion
    }
}
