using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public abstract class BaseSceneObject : MonoBehaviour
    {
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;
        public SceneVariablesSO SceneVariablesSO => sceneVariablesSO;

        #region Base
        private void Awake()
        {
            Init();
            UpdateBelongings();

            Awake_Ext();
        }

        private void OnEnable()
        {
            //SceneState.Register(this);

            RegisterElements();

            OnEnable_Ext();
        }
        private void OnDisable()
        {
            //SceneState.Unregister(this);

            UnregisterElements();

            OnDisable_Ext();
        }

        private void OnValidate()
        {
            UpdateSceneVariables();

            OnValidate_Ext();
        }
        #endregion

        #region Abstracts
        /// <summary>
        /// Called on <see cref="Awake"/>.<br></br>
        /// Init <see cref="SceneState.IInitializable"/>s elements HERE.
        /// </summary>
        protected abstract void Init();
        /// <summary>
        /// Called on <see cref="Awake"/>.<br></br>
        /// Update the belongings of <see cref="SceneState.ISceneObjectBelongable"/>s elements to this object HERE.
        /// </summary>
        protected abstract void UpdateBelongings();
        /// <summary>
        /// Called on <see cref="OnValidate"/>.<br></br>
        /// Update the <see cref="Dhs5.SceneCreation.SceneVariablesSO"/> of <see cref="SceneState.ISceneVarSetupable"/> and <see cref="SceneState.ISceneVarTypedSetupable"/> elements HERE.
        /// </summary>
        protected abstract void UpdateSceneVariables();
        /// <summary>
        /// Called on <see cref="OnEnable"/>.<br></br>
        /// Register <see cref="SceneState.ISceneRegisterable"/>s elements HERE.
        /// </summary>
        protected abstract void RegisterElements();
        /// <summary>
        /// Called on <see cref="OnDisable"/>.<br></br>
        /// Unregister <see cref="SceneState.ISceneRegisterable"/>s elements HERE.
        /// </summary>
        protected abstract void UnregisterElements();
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

        #region SceneObject's specifics
        /// <summary>
        /// Called on <see cref="SceneManager.Start"/> once the <see cref="SceneState"/> has been set up.
        /// </summary>
        public virtual void OnStartScene() { }
        #endregion

        #region Utility

        #region Setup
        protected void Setup(SceneState.ISceneVarSetupable var)
        {
            var.SetUp(sceneVariablesSO);
        }
        protected void Setup(List<SceneState.ISceneVarSetupable> vars)
        {
            vars.SetUp(sceneVariablesSO);
        }
        protected void Setup(params List<SceneState.ISceneVarSetupable>[] vars)
        {
            foreach (var var in vars)
                var.SetUp(sceneVariablesSO);
        }
        #endregion

        #region Belong
        protected void Belong(SceneState.ISceneObjectBelongable var)
        {
            //var.BelongTo(this);
        }
        protected void Belong(List<SceneState.ISceneObjectBelongable> vars)
        {
            //vars.BelongTo(this);
        }
        protected void Belong(params List<SceneState.ISceneObjectBelongable>[] vars)
        {
            //foreach (var var in vars)
            //    var.BelongTo(this);
        }
        #endregion

        #endregion
    }
}
