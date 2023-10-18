using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dhs5.SceneCreation
{
    public class SceneManager : SceneObject
    {
        #region Singleton
        public static SceneManager Instance { get; private set; }
        protected override void Awake_Ext()
        {
            base.Awake_Ext();

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        #endregion

        [Header("Manager")]
        [Tooltip("Events called when the Scene starts,\n just before every SceneObject.OnStartScene()")]
        [SerializeField] protected List<SceneEvent> onSceneStart;
        [Tooltip("Events called when the Scene is going to change,\n just before every SceneObject.OnChangeScene()")]
        [SerializeField] protected List<SceneEvent> onSceneChange;
        [Tooltip("Events called on GameOver,\n just before every SceneObject.OnGameOver()")]
        [SerializeField] protected List<SceneEvent> onGameOver;

        /// <summary>
        /// Event called when the Scene starts for non-<see cref="SceneObject"/> elements to subscribe
        /// </summary>
        public static event Action SceneStartEvent;
        /// <summary>
        /// Event called when the Scene is going to change for non-<see cref="SceneObject"/> elements to subscribe
        /// </summary>
        public static event Action SceneChangeEvent;
        /// <summary>
        /// Event called on GameOver for non-<see cref="SceneObject"/> elements to subscribe
        /// </summary>
        public static event Action GameOverEvent;

        protected virtual void Start()
        {
            SetBalancingIndex();

            SetSceneVars();

            StartScene();
        }

        #region SceneObject Extension
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            onSceneStart.SetUp(sceneVariablesSO);
            onSceneChange.SetUp(sceneVariablesSO);
        }
        protected override void RegisterElements()
        {
            base.RegisterElements();

            Register(nameof(onSceneStart), onSceneStart);
            Register(nameof(onSceneChange), onSceneChange);
        }
        #endregion


        #region Scene Main Events
        protected virtual void StartScene()
        {
            onSceneStart.Trigger();
            SceneState.StartScene();

            SceneStartEvent?.Invoke();
        }
        public virtual void ChangeScene()
        {
            onSceneChange.Trigger();
            SceneState.ChangeScene();

            SceneChangeEvent?.Invoke();
        }
        public virtual void GameOver()
        {
            onGameOver.Trigger();
            SceneState.GameOver();

            GameOverEvent?.Invoke();
        }
        #endregion

        #region Scenes Management

        #endregion

        #region SceneVars Setup
        /// <summary>
        /// Set the <see cref="SceneState"/>'s SceneVars at the beginning of the Scene
        /// </summary>
        protected void SetSceneVars()
        {
            SceneState.SetSceneVars(sceneVariablesSO, BalancingIndex);
        }
        #endregion

        #region Balancing Setup
        /// <summary>
        /// Index of the balancing sheet to use for this scene.<br/>
        /// <b>0 is the base SceneVariablesSO, 1 is the first BalancingSheet of the list</b>
        /// </summary>
        public static int BalancingIndex { get; protected set; } = 0;

        /// <summary>
        /// Function called in <see cref="Start"/> just before <see cref="SetSceneVars"/>.<br></br>
        /// By default : <c>BalancingIndex = IntersceneState.BalancingLevel;</c><br></br><br></br>
        /// Override this function to change the <see cref="BalancingIndex"/>.
        /// </summary>
        protected virtual void SetBalancingIndex()
        {
            BalancingIndex = IntersceneState.BalancingLevel;
        }

        /// <summary>
        /// Update the balancing during a Scene <b>ARLEADY SETUP</b><br/>
        /// and update <b>ONLY the STATIC and RANDOM vars</b><br/>
        /// without triggering any <i>ChangedVar event</i>.<br></br>
        /// Only the balancing of this scene will be changed.
        /// </summary>
        /// <remarks>Never use before <see cref="SetSceneVars"/></remarks>
        /// <param name="balancingIndex">Index of the balancing sheet to apply</param>
        public void UpdateBalancing(int balancingIndex)
        {
            BalancingIndex = balancingIndex;
            SceneState.ActuBalancing(sceneVariablesSO, BalancingIndex);
        }
        #endregion

        #region Editor
        /// <summary>
        /// !!! EDITOR FUNCTION !!! 
        /// Do not use at runtime !
        /// </summary>
        /// <param name="_sceneVariablesSO"></param>
        public void SetSceneVariablesSO(SceneVariablesSO _sceneVariablesSO)
        {
#if UNITY_EDITOR
            sceneVariablesSO = _sceneVariablesSO;
#endif
        }
        #endregion
    }
}
