using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

            Awake_Ext();
        }

        private void OnEnable()
        {
            //SceneState.Register(this);

            StartSubscription();

            OnEnable_Ext();
        }
        private void OnDisable()
        {
            //SceneState.Unregister(this);

            EndSubscription();

            OnDisable_Ext();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();

            OnValidate_Ext();
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
        #endregion

        #region Abstracts
        /// <summary>
        /// Called on <see cref="OnValidate"/>.<br></br>
        /// Update the <see cref="Dhs5.SceneCreation.SceneVariablesSO"/> of <see cref="SceneState.ISceneVarSetupable"/> and <see cref="SceneState.ISceneVarTypedSetupable"/> elements HERE.
        /// </summary>
        protected abstract void UpdateSceneVariables();
        /// <summary>
        /// Called on <see cref="Awake"/>.<br></br>
        /// Register in this function :<list type="bullet">
        /// <item>
        /// <see cref="BaseSceneEvent"/> lists with <see cref="RegisterEvent{T}(string, List{T})"/>
        /// </item>
        /// <item>
        /// <see cref="BaseSceneListener"/> lists with <see cref="RegisterListener{T}(string, List{T})"/>
        /// </item>
        /// <item>
        /// <see cref="SceneVarTween"/>s with <see cref="RegisterTween(string, SceneVarTween)"/>
        /// </item>
        /// </list>
        /// </summary>
        protected abstract void RegisterSceneElements();
        #endregion

        #region Extensions
        /// <summary>
        /// <see cref="Awake"/> extension.
        /// </summary>
        protected virtual void Awake_Ext() { }
        /// <summary>
        /// <see cref="OnValidate"/> extension.
        /// </summary>
        protected virtual void OnValidate_Ext() { }
        /// <summary>
        /// <see cref="OnEnable"/> extension.
        /// </summary>
        protected virtual void OnEnable_Ext() { }
        /// <summary>
        /// <see cref="OnDisable"/> extension.
        /// </summary>
        protected virtual void OnDisable_Ext() { }
        #endregion

        #region Registration
        protected Dictionary<string, List<BaseSceneEvent>> SceneEventsDico { get; private set; } = new();
        protected Dictionary<string, List<BaseSceneListener>> SceneListenersDico { get; private set; } = new();
        protected Dictionary<string, SceneVarTween> TweensDico { get; private set; } = new();

        protected void RegisterEvent<T>(string name, List<T> list) where T : BaseSceneEvent
        {
            SceneEventsDico[name] = list.Cast<BaseSceneEvent>().ToList();
        }
        protected void RegisterListener<T>(string name, List<T> list) where T : BaseSceneListener
        {
            SceneListenersDico[name] = list.Cast<BaseSceneListener>().ToList();
        }
        protected void RegisterTween(string name, SceneVarTween tween)
        {
            TweensDico[name] = tween;
        }

        #region Utility
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

        #endregion

        #region SceneObject's specifics
        /// <summary>
        /// Called on <see cref="SceneManager.Start"/> once the <see cref="SceneState"/> has been set up.
        /// </summary>
        public virtual void OnStartScene() { }
        #endregion

        #region Utility

        #region Setup
        protected void Setup<T>(T var) where T : SceneState.ISceneVarSetupable
        {
            var.SetUp(sceneVariablesSO);
        }
        protected void Setup<T>(List<T> vars) where T : SceneState.ISceneVarSetupable
        {
            vars.SetUp(sceneVariablesSO);
        }
        protected void Setup<T>(params List<T>[] vars) where T : SceneState.ISceneVarSetupable
        {
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
            //vars.BelongTo(this);
        }
        protected void Belong<T>(params List<T>[] vars) where T : SceneState.ISceneObjectBelongable
        {
            //foreach (var var in vars)
            //    var.BelongTo(this);
        }
        #endregion

        #endregion
    }
}
