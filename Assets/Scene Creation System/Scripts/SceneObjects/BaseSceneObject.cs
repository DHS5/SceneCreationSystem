using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Scripting;
using System.Text;

namespace Dhs5.SceneCreation
{
    public abstract class BaseSceneObject : MonoBehaviour
    {
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;
        public SceneVariablesSO SceneVariablesSO => sceneVariablesSO;

        #region Base
        private void Awake()
        {
            RegisterSceneElements();

            Init();
            SetBelongings();

            OnSceneObjectAwake();
        }

        private void OnEnable()
        {
            //SceneState.Register(this);

            StartSubscription();
            EnableScriptables();

            OnSceneObjectEnable();
        }
        private void OnDisable()
        {
            //SceneState.Unregister(this);

            EndSubscription();
            DisableScriptables();

            OnSceneObjectDisable();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();

            OnSceneObjectValidate();
        }
        #endregion

        #region Automatisation
        /// <summary>
        /// Called on <see cref="Awake"/>.<br></br><br></br>
        /// If overriden :<br></br>
        /// ALWAYS call <see cref="Init"/><br></br>
        /// Init UNREGISTERED <see cref="SceneState.IInitializable"/>s elements HERE.
        /// </summary>
        protected virtual void Init()
        {
            LinkScriptables();
            InitEvents();
        }
        /// <summary>
        /// Called on <see cref="Awake"/>.<br></br><br></br>
        /// If overriden :<br></br>
        /// ALWAYS call <see cref="SetBelongings"/><br></br>
        /// Set the belongings of UNREGISTERED <see cref="SceneState.ISceneObjectBelongable"/>s elements to this object HERE.
        /// </summary>
        protected virtual void SetBelongings()
        {
            SetEventsBelongings();
            SetListenersBelongings();
            SetTweensBelongings();
        }
        /// <summary>
        /// Called on <see cref="OnEnable"/>.<br></br><br></br>
        /// If overriden :<br></br>
        /// ALWAYS call <see cref="StartSubscription"/><br></br>
        /// Start subscription of UNREGISTERED <see cref="SceneState.ISceneSubscribable"/>s elements HERE.
        /// </summary>
        protected virtual void StartSubscription()
        {
            StartListenersSubscription();
        }
        /// <summary>
        /// Called on <see cref="OnDisable"/>.<br></br><br></br>
        /// If overriden :<br></br>
        /// ALWAYS call <see cref="EndSubscription"/><br></br>
        /// End subscription of UNREGISTERED <see cref="SceneState.ISceneSubscribable"/>s elements HERE.
        /// </summary>
        protected virtual void EndSubscription()
        {
            EndListenersSubscription();
        }
        /// <summary>
        /// Called on <see cref="OnEnable"/>.<br></br><br></br>
        /// If overriden :<br></br>
        /// ALWAYS call <see cref="EnableScriptables"/><br></br>
        /// Enable only UNREGISTERED <see cref="SceneScriptableObject"/>s HERE.
        /// </summary>
        protected virtual void EnableScriptables()
        {
            EnableScriptablesList();
        }
        /// <summary>
        /// Called on <see cref="OnDisable"/>.<br></br><br></br>
        /// If overriden :<br></br>
        /// ALWAYS call <see cref="DisableScriptables"/><br></br>
        /// Disable only UNREGISTERED <see cref="SceneScriptableObject"/>s HERE.
        /// </summary>
        protected virtual void DisableScriptables()
        {
            DisableScriptablesList();
        }
        #endregion

        #region Abstracts
        /// <summary>
        /// Called on <see cref="OnValidate"/>.<br></br>
        /// Update in this function the <see cref="Dhs5.SceneCreation.SceneVariablesSO"/> of :
        /// <list type="bullet">
        /// <item>
        /// <see cref="SceneState.ISceneVarSetupable"/> elements
        /// </item>
        /// <item>
        /// <see cref="SceneState.ISceneVarTypedSetupable"/> elements
        /// </item>
        /// </list>
        /// </summary>
        protected abstract void UpdateSceneVariables();
        /// <summary>
        /// Called on <see cref="Awake"/>.<br></br>
        /// Register in this function :
        /// <list type="bullet">
        /// <item>
        /// <see cref="BaseSceneEvent"/> lists with <see cref="RegisterEvent{T}(string, List{T})"/>
        /// </item>
        /// <item>
        /// <see cref="BaseSceneListener"/> lists with <see cref="RegisterListener{T}(string, List{T})"/>
        /// </item>
        /// <item>
        /// <see cref="SceneVarTween"/>s with <see cref="RegisterTween(string, SceneVarTween)"/>
        /// </item>
        /// <item>
        /// <see cref="SceneScriptableObject"/>s with <see cref="RegisterScriptable{T}(T)"/>
        /// </item>
        /// </list>
        /// </summary>
        protected abstract void RegisterSceneElements();
        #endregion

        #region Extensions
        /// <summary>
        /// <see cref="Awake"/> extension.
        /// </summary>
        protected virtual void OnSceneObjectAwake() { }
        /// <summary>
        /// <see cref="OnValidate"/> extension.
        /// </summary>
        protected virtual void OnSceneObjectValidate() { }
        /// <summary>
        /// <see cref="OnEnable"/> extension.
        /// </summary>
        protected virtual void OnSceneObjectEnable() { }
        /// <summary>
        /// <see cref="OnDisable"/> extension.
        /// </summary>
        protected virtual void OnSceneObjectDisable() { }
        #endregion

        #region Registration
        protected Dictionary<string, List<BaseSceneEvent>> SceneEventsDico { get; private set; } = new();
        protected Dictionary<string, List<BaseSceneListener>> SceneListenersDico { get; private set; } = new();
        protected Dictionary<string, SceneVarTween> TweensDico { get; private set; } = new();
        protected List<SceneScriptableObject> SceneScriptablesList { get; private set; } = new();

        #region Events
        protected void RegisterEvent<T>(string name, List<T> list) where T : BaseSceneEvent
        {
            SceneEventsDico[name] = list.Cast<BaseSceneEvent>().ToList();
        }
        protected void RegisterEvents<T>(params (string name, List<T> list)[] vars) where T : BaseSceneEvent
        {
            if (vars.IsValid())
                foreach (var v in vars)
                    RegisterEvent(v.name, v.list);
        }
        #endregion

        #region Listeners
        protected void RegisterListener<T>(string name, List<T> list) where T : BaseSceneListener
        {
            SceneListenersDico[name] = list.Cast<BaseSceneListener>().ToList();
        }
        protected void RegisterListeners<T>(params (string name, List<T> list)[] vars) where T : BaseSceneListener
        {
            if (vars.IsValid())
                foreach (var v in vars)
                    RegisterListener(v.name, v.list);
        }
        #endregion

        #region Tweens
        protected void RegisterTween(string name, SceneVarTween tween)
        {
            TweensDico[name] = tween;
        }
        protected void RegisterTweens(params (string name, SceneVarTween tween)[] vars)
        {
            if (vars.IsValid())
                foreach (var v in vars)
                    RegisterTween(v.name, v.tween);
        }
        #endregion

        #region Scriptables
        protected void RegisterScriptable<T>(T scriptable) where T : SceneScriptableObject
        {
            if (SceneScriptablesList.Contains(scriptable)) return;

            SceneScriptablesList.Add(scriptable);
        }
        protected void RegisterScriptables<T>(List<T> scriptables) where T : SceneScriptableObject
        {
            if (scriptables.IsValid())
                foreach (var scriptable in scriptables)
                    RegisterScriptable(scriptable);
        }
        protected void RegisterScriptables<T>(params T[] scriptables) where T : SceneScriptableObject
        {
            if (scriptables.IsValid())
                foreach (var scriptable in scriptables)
                    RegisterScriptable(scriptable);
        }
        #endregion

        #region Utility

        #region Events
        private void InitEvents()
        {
            if (SceneEventsDico.IsValid())
            {
                foreach (var pair in SceneEventsDico)
                {
                    pair.Value.Init();
                }
            }
        }
        private void SetEventsBelongings()
        {
            if (SceneEventsDico.IsValid())
            {
                foreach (var pair in SceneEventsDico)
                {
                    Belong(pair.Value);
                }
            }
        }
        #endregion

        #region Listeners
        private void SetListenersBelongings()
        {
            if (SceneListenersDico.IsValid())
            {
                foreach (var pair in SceneListenersDico)
                {
                    Belong(pair.Value);
                }
            }
        }
        private void StartListenersSubscription()
        {
            if (SceneListenersDico.IsValid())
            {
                foreach (var pair in SceneListenersDico)
                {
                    pair.Value.Subscribe();
                }
            }
        }
        private void EndListenersSubscription()
        {
            if (SceneListenersDico.IsValid())
            {
                foreach (var pair in SceneListenersDico)
                {
                    pair.Value.Unsubscribe();
                }
            }
        }
        #endregion

        #region Tweens
        private void SetTweensBelongings()
        {
            if (TweensDico.IsValid())
            {
                foreach (var pair in TweensDico)
                {
                    Belong(pair.Value);
                }
            }
        }
        #endregion

        #region Scriptables
        private void LinkScriptables()
        {
            if (SceneScriptablesList.IsValid())
            {
                Link(SceneScriptablesList);
            }
        }
        private void EnableScriptablesList()
        {
            if (SceneScriptablesList.IsValid())
            {
                SceneScriptablesList.OnSceneObjectEnable();
            }
        }
        private void DisableScriptablesList()
        {
            if (SceneScriptablesList.IsValid())
            {
                SceneScriptablesList.OnSceneObjectDisable();
            }
        }
        #endregion

        #endregion

        #endregion

        #region SceneObject's specifics
        public abstract bool DoStartScene { get; }
        /// <summary>
        /// Called on <see cref="SceneManager.StartScene"/> once the <see cref="SceneState"/> has been set up.
        /// </summary>
        /// <remarks>Must set <see cref="DoStartScene"/> to TRUE to be called.</remarks>
        public virtual void OnStartScene() { }

        public abstract bool DoChangeScene { get; }
        /// <summary>
        /// Called on <see cref="SceneManager.ChangeScene"/> when the Scene is going to change.
        /// </summary>
        /// <remarks>Must set <see cref="DoChangeScene"/> to TRUE to be called.</remarks>
        public virtual void OnChangeScene() { }

        public abstract bool DoGameOver { get; }
        /// <summary>
        /// Called on <see cref="SceneManager.GameOver"/> when the game is over.
        /// </summary>
        /// <remarks>Must set <see cref="DoGameOver"/> to TRUE to be called.</remarks>
        public virtual void OnGameOver() { }
        #endregion

        #region Profiles
        protected List<SceneProfile> SceneProfiles { get; private set; } = new();
        public int ProfileCount => SceneProfiles.Count;

        #region List Handling
        public void ApplyProfiles(SceneVariablesSO _sceneVariablesSO, List<SceneProfile> profiles)
        {
            sceneVariablesSO = _sceneVariablesSO;

            if (!profiles.IsValid()) return;

            ClearProfiles();

            SceneProfiles.AddRange(profiles);

            //profiles.Attach(this);
        }
        public void AddProfile(SceneProfile profile)
        {
            SceneProfiles ??= new();

            SceneProfiles.Add(profile);
        }
        public bool RemoveProfile(SceneProfile profile)
        {
            if (!SceneProfiles.IsValid()) return false;

            if (SceneProfiles.Contains(profile))
            {
                profile.Detach();
                return SceneProfiles.Remove(profile);
            }
            return false;
        }
        public bool RemoveProfileOfType<T>(bool all = false) where T : SceneProfile
        {
            if (!SceneProfiles.IsValid()) return false;

            bool result = false;

            foreach (var profile in SceneProfiles.Copy())
            {
                if (profile is T p)
                {
                    p.Detach();
                    SceneProfiles.Remove(p);
                    if (!all) return true;
                    result = true;
                }
            }

            return result;
        }

        public void ClearProfiles()
        {
            if (!SceneProfiles.IsValid())
            {
                SceneProfiles = new();
                return;
            }

            SceneProfiles.Detach();
            SceneProfiles.Clear();
        }
        #endregion

        #region Get Profile
        public SceneProfile GetProfileAtIndex(int index)
        {
            if (!SceneProfiles.IsValid() || !SceneProfiles.IsIndexValid(index)) return null;

            return SceneProfiles[index];
        }
        public T GetProfileOfType<T>() where T : SceneProfile
        {
            if (!SceneProfiles.IsValid()) return null;

            return SceneProfiles.Find(p => p is T) as T;
        }
        public bool HasProfileOfType<T>() where T : SceneProfile
        {
            if (!SceneProfiles.IsValid()) return false;

            if (SceneProfiles.Find(p => p is T) != null) return true;
            return false;
        }
        #endregion

        #region Actions
        public bool TriggerProfileOfType<T>(bool all = false) where T : SceneProfile
        {
            if (!SceneProfiles.IsValid()) return false;

            bool result = false;

            foreach (var profile in SceneProfiles)
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
            if (!SceneProfiles.IsValid()) return false;

            foreach (var p in SceneProfiles)
                if (p == profile)
                {
                    p.TriggerProfile();
                    return true;
                }
            return false;
        }
        public void TriggerAllProfiles()
        {
            if (!SceneProfiles.IsValid()) return;

            foreach (var profile in SceneProfiles)
                profile.TriggerProfile();
        }
        public void TriggerEventInProfiles(string eventID)
        {
            if (!SceneProfiles.IsValid()) return;

            foreach (var profile in SceneProfiles)
                profile.TriggerEventInProfile(eventID);
        }
        [Preserve]
        public void TriggerEventInProfilesAndRemove(string eventID, int triggerNumber)
        {
            if (!SceneProfiles.IsValid()) return;

            foreach (var profile in SceneProfiles)
                profile.TriggerEventInProfileAndRemove(eventID, triggerNumber);
        }
        public void TriggerAllEventsInProfilesAndRemove(int triggerNumber)
        {
            if (!SceneProfiles.IsValid()) return;

            foreach (var profile in SceneProfiles)
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
            if (!SceneProfiles.IsValid()) return false;

            foreach (var profile in SceneProfiles)
                if (profile is T p)
                {
                    return p.TriggerProfileRandom(filter);
                }
            return false;
        }
        public bool TriggerRandomInProfileOfTypeAndRemove<T>(string filter = null) where T : SceneProfile
        {
            if (!SceneProfiles.IsValid()) return false;

            foreach (var profile in SceneProfiles)
                if (profile is T p)
                {
                    return p.TriggerProfileRandom(filter, true);
                }
            return false;
        }

        #endregion

        #endregion

        #region Utility

        #region Setup
        protected void Setup<T>(T var) where T : SceneState.ISceneVarSetupable
        {
            var.SetUp(sceneVariablesSO);
        }
        protected void Setup<T>(List<T> vars) where T : SceneState.ISceneVarSetupable
        {
            if (vars.IsValid())
                vars.SetUp(sceneVariablesSO);
        }
        protected void Setup<T>(params List<T>[] vars) where T : SceneState.ISceneVarSetupable
        {
            if (vars.IsValid())
                foreach (var var in vars)
                    var.SetUp(sceneVariablesSO);
        }
        #endregion

        #region Belong
        protected void Belong<T>(T var) where T : SceneState.ISceneObjectBelongable
        {
            //var.BelongTo(this);
        }
        protected void Belong<T>(List<T> vars) where T : SceneState.ISceneObjectBelongable
        {
            //if (vars.IsValid())
            //  vars.BelongTo(this);
        }
        protected void Belong<T>(params List<T>[] vars) where T : SceneState.ISceneObjectBelongable
        {
            if (vars.IsValid())
                foreach (var var in vars)
                    Belong(var);
        }
        #endregion

        #region Link
        protected void Link<T>(T scriptable) where T : SceneScriptableObject
        {
            //scriptable.Link(this);
        }
        protected void Link<T>(List<T> scriptables) where T : SceneScriptableObject
        {
            //if (scriptables.IsValid())
            //    scriptables.Link(this);
        }
        protected void Link<T>(params List<T>[] vars) where T : SceneScriptableObject
        {
            if (vars.IsValid())
                foreach (var scriptables in vars)
                    Link(scriptables);
        }
        #endregion

        #endregion



        #region SceneClock Actions

        /// <summary>
        /// Starts the timeline named <paramref name="timelineID"/> from first step (step 0)
        /// </summary>
        /// <param name="timelineID">ID of the timeline to start</param>
        [Preserve]
        public void Clock_StartTimeline(string timelineID)
        {
            SceneClock.Instance.StartTimeline(timelineID);
        }
        /// <summary>
        /// Starts the timeline named <paramref name="timelineID"/> from step <paramref name="step"/>
        /// </summary>
        /// <param name="timelineID">ID of the timeline to start</param>
        /// <param name="step">Index of the step in the <b>TimelineObject</b> list of the <b>SceneTimeline</b></param>
        [Preserve]
        public void Clock_StartTimeline(string timelineID, int step)
        {
            SceneClock.Instance.StartTimeline(timelineID, step);
        }

        /// <summary>
        /// Stops the timeline named <paramref name="timelineID"/>
        /// </summary>
        /// <param name="timelineID">ID of the timeline to stop</param>
        [Preserve]
        public void Clock_StopTimeline(string timelineID)
        {
            SceneClock.Instance.StopTimeline(timelineID);
        }

        /// <summary>
        /// Makes the timeline <paramref name="timelineID"/> go to step <paramref name="step"/>.<br/>
        /// If <paramref name="interrupt"/> is <b>true</b>, immediatly go to new step.<br/>
        /// Else, wait for the current step to finish its execution.
        /// </summary>
        /// <param name="timelineID">ID of the timeline to change step</param>
        /// <param name="step">Index of the step in the <b>TimelineObject</b> list of the <b>SceneTimeline</b></param>
        /// <param name="interrupt">Whether to interrupt the execution of the current step</param>
        [Preserve]
        public void Clock_TimelineGoToStep(string timelineID, int step, bool interrupt)
        {
            SceneClock.Instance.GoToStep(timelineID, step, interrupt);
        }
        #endregion
    }
}
