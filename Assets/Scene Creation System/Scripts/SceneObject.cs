using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System.Text;
using System.Linq;

namespace Dhs5.SceneCreation
{
    [DisallowMultipleComponent]
    public class SceneObject : MonoBehaviour, SceneState.ISceneVarDependant
    {
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;
        public SceneVariablesSO SceneVariablesSO => sceneVariablesSO;

        [Header("Listeners")]
        [SerializeField] protected List<SceneListener> sceneListeners;

        [Header("Actions")]
        [SerializeField] protected List<SceneEvent<SceneEventParam>> sceneEvents;

        #region Update Listeners, Actions & Tweens
        private void Awake()
        {
            LinkSceneScriptables();
            Init();
            UpdateBelongings();

            Awake_Ext();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();

            OnValidate_Ext();
        }

        // Virtuals

        protected virtual void Init()
        {
            sceneEvents.Init();

            RegisterElements();

            if (SceneEventsDico != null && SceneEventsDico.Count > 0)
            {
                foreach (var pair in SceneEventsDico)
                {
                    pair.Value.Init();
                }
            }
        }
        protected virtual void UpdateBelongings()
        {
            sceneListeners.BelongTo(this);
            sceneEvents.BelongTo(this);

            if (SceneEventsDico != null && SceneEventsDico.Count > 0)
            {
                foreach (var pair in SceneEventsDico)
                {
                    pair.Value.BelongTo(this);
                }
            }
            if (TweenDico != null && TweenDico.Count > 0)
            {
                foreach (var pair in TweenDico)
                {
                    pair.Value.BelongTo(this);
                }
            }
        }
        protected virtual void UpdateSceneVariables()
        {
            sceneListeners.SetUp(sceneVariablesSO);
            sceneEvents.SetUp(sceneVariablesSO);
        }
        protected virtual void Awake_Ext() { }
        public virtual void OnStartScene() { }
        protected virtual void OnValidate_Ext() { }
        #endregion

        #region Scene Listeners registration
        private void OnEnable()
        {
            SceneState.Register(this);

            sceneScriptables.OnSceneObjectEnable();

            sceneListeners.Register();

            OnEnable_Ext();
        }
        private void OnDisable()
        {
            SceneState.Unregister(this);

            sceneScriptables.OnSceneObjectDisable();

            sceneListeners.Unregister();

            OnDisable_Ext();
        }

        protected virtual void OnEnable_Ext() { }
        protected virtual void OnDisable_Ext() { }
        #endregion

        #region Scene Events Registration
        protected Dictionary<string, List<BaseSceneEvent>> SceneEventsDico { get; private set; }
        protected Dictionary<string, SceneVarTween> TweenDico { get; private set; }
        /// <summary>
        /// Function where all <see cref="BaseSceneEvent"/> and <see cref="SceneVarTween"/> lists should be registered with : <see cref="Register{T}(string, List{T})"/>
        /// </summary>
        protected virtual void RegisterElements()
        {
            SceneEventsDico = new();
            TweenDico = new();
        }

        protected void Register<T>(string name, List<T> events) where T : BaseSceneEvent
        {
            SceneEventsDico[name] = events.Cast<BaseSceneEvent>().ToList();
        }
        protected void Register(string name, SceneVarTween tween)
        {
            TweenDico[name] = tween;
        }
        #endregion

        #region Scriptable Link
        private List<SceneScriptableObject> sceneScriptables = new();

        protected virtual void LinkSceneScriptables() { }

        protected void Link<T>(T sso) where T : SceneScriptableObject
        {
            sso.Link(this);
            sceneScriptables.Add(sso);
        }
        protected void Link<T>(List<T> ssos) where T : SceneScriptableObject
        {
            if (ssos.IsValid()) 
                foreach (var sso in ssos)
                    Link(sso);
        }
        protected void Link<T>(params List<T>[] ssos) where T : SceneScriptableObject
        {
            if (ssos.IsValid())
                foreach (var sso in ssos)
                    Link(sso);
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
        public void ApplyProfiles(SceneVariablesSO _sceneVariablesSO, List<SceneProfile> profiles)
        {
            sceneVariablesSO = _sceneVariablesSO;

            if (profiles == null || profiles.Count <= 0) return;

            ClearProfiles();

            sceneProfiles.AddRange(profiles);
            profiles.Attach(this);
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

            sceneProfiles.Detach();
            sceneProfiles.Clear();
        }
        #endregion

        #region Get Profile
        public T GetProfileOfType<T>() where T : SceneProfile
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return null;

            return sceneProfiles.Find(p => p is T) as T;
        }
        public bool HasProfileOfType<T>() where T : SceneProfile
        {
            if (sceneProfiles == null || sceneProfiles.Count <= 0) return false;

            if (sceneProfiles.Find(p => p is T) != null) return true;
            return false;
        }
        #endregion

        #region Profile Override
        public bool OverrideListeners(SceneProfile profile, List<SceneListener> listeners)
        {
            if (profile == null || !profile.CanOverrideListeners || listeners == null) return false;

            sceneListeners.AddRange(listeners);
            return true;
        }
        public bool OverrideEvents(SceneProfile profile, List<SceneEvent<SceneEventParam>> events)
        {
            if (profile == null || !profile.CanOverrideEvents || events == null) return false;

            sceneEvents.AddRange(events);
            return true;
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
        public List<string> LogLines(bool detailed = false, bool showEmpty = false)
        {
            string passToLine = "Line()";
            List<string> lines = new();
            StringBuilder sb = new();

            AppendColor(SceneLogger.SceneObjectColor, "--------------------------------------------------");
            Line();

            if (showEmpty || sceneListeners.IsValid())
            {
                AppendColor(SceneLogger.ListenerColor, "Listeners :");
                Line();

                if (detailed)
                {
                    AppendColor(SceneLogger.ListenerColor, "----------------------------------------");
                    Line();
                }

                foreach (var listener in sceneListeners)
                {
                    lines.AddRange(listener.LogLines(detailed));
                }

                if (detailed)
                {
                    AppendColor(SceneLogger.ListenerColor, "----------------------------------------");
                    Line();
                }
            }

            if (showEmpty || sceneEvents.IsValid())
            {
                AppendColor(SceneLogger.EventColor, "Events :");
                Line();

                if (detailed)
                {
                    AppendColor(SceneLogger.EventColor, "----------------------------------------");
                    Line();
                }

                foreach (var events in sceneEvents)
                {
                    lines.AddRange(events.LogLines(detailed));
                }

                if (detailed)
                {
                    AppendColor(SceneLogger.EventColor, "----------------------------------------");
                    Line();
                }
            }


            ChildLog(lines, sb, detailed);

            RegisterElements();
            if (showEmpty || SceneEventsDico.IsReallyValid())
            {
                AppendColor(SceneLogger.ExtensionEventColor, "Extension Events :", passToLine, "----------------------------------------");
                Line();

                foreach (var pair in SceneEventsDico)
                {
                    if (!showEmpty && !pair.Value.IsValid()) continue;
                    AppendColor(SceneLogger.ExtensionEventColor, "   * ");
                    AppendBold(pair.Key);
                    Line();

                    if (!pair.Value.IsValid()) continue;

                    foreach (var e in pair.Value)
                        lines.AddRange(e.LogLines(detailed, "      "));
                }
                AppendColor(SceneLogger.ExtensionEventColor, "----------------------------------------");
                Line();
            }
            if (showEmpty || TweenDico.IsValid())
            {
                AppendColor(SceneLogger.TweenColor, "SceneVarTweens :", passToLine, "----------------------------------------");
                Line();

                foreach (var pair in TweenDico)
                {
                    AppendColor(SceneLogger.TweenColor, "   * ");
                    AppendBold(pair.Key);
                    sb.Append(" : ");
                    sb.Append(pair.Value.LogString());
                    Line();
                }

                AppendColor(SceneLogger.TweenColor, "----------------------------------------", passToLine);
            }

            AppendColor(SceneLogger.SceneObjectColor, "--------------------------------------------------");
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
            void AppendColor(string color, params string[] strings)
            {
                sb.Append(color);
                foreach (var s in strings)
                {
                    if (s == passToLine) Line();
                    else sb.Append(s);
                }
                sb.Append(SceneLogger.ColorEnd);
            }
            void AppendBold(params string[] strings)
            {
                sb.Append(SceneLogger.Bold);
                foreach (var s in strings)
                {
                    if (s == passToLine) Line();
                    else sb.Append(s);
                }
                sb.Append(SceneLogger.BoldEnd);
            }
            #endregion
        }

        protected virtual void ChildLog(List<string> lines, StringBuilder sb, bool detailed) { }
        #endregion

        #region Dependencies
        public List<int> Dependencies 
        { 
            get
            {
                List<int> dependencies = new List<int>();

                dependencies.AddRange(sceneListeners.Dependencies());
                dependencies.AddRange(sceneEvents.Dependencies());

                RegisterElements();
                if (SceneEventsDico.IsReallyValid())
                {
                    foreach (var pair in SceneEventsDico)
                    {
                        if (pair.Value.IsValid())
                            dependencies.AddRange(pair.Value.Dependencies());
                    }
                }
                if (TweenDico.IsValid())
                {
                    foreach (var pair in TweenDico)
                    {
                        dependencies.AddRange(pair.Value.Dependencies);
                    }
                }

                dependencies.AddRange(ChildDependencies());

                return dependencies;
            }
        }
        public bool DependOn(int UID) { return Dependencies.Contains(UID); }
        public void SetForbiddenUID(int UID) { }

        protected virtual List<int> ChildDependencies() { return new(); }
        #endregion

        #region Editor
        /// <summary>
        /// !!! EDITOR FUNCTION !!!
        /// </summary>
        [ContextMenu("Get SceneVariablesSO")]
        public void GetSceneVariablesSOInScene()
        {
#if UNITY_EDITOR
            if (Application.isPlaying || this is SceneManager) return;
            SceneManager manager = FindObjectOfType<SceneManager>();
            if (manager != null && manager != this)
            {
                sceneVariablesSO = manager.sceneVariablesSO;

                UpdateSceneVariables();
            }
#endif
        }
        #endregion

        #region Utility
        public bool IsEmpty()
        {
            if (sceneListeners.IsValid()) return false;
            if (sceneEvents.IsValid()) return false;
            RegisterElements();
            if (SceneEventsDico.IsReallyValid()) return false;
            if (TweenDico.IsValid()) return false;
            if (!ChildIsEmpty()) return false;

            return true;
        }
        protected virtual bool ChildIsEmpty() { return true; }
        #endregion
    }
}
