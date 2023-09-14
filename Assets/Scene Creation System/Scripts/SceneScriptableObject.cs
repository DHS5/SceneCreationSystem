using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public abstract class SceneScriptableObject : ScriptableObject
    {
        [SerializeField, HideInInspector] protected SceneVariablesSO sceneVariablesSO;
        public SceneVariablesSO SceneVariablesSO => sceneVariablesSO;

        [SerializeField, HideInInspector] protected SceneObject sceneObject;

        #region Link
        public void Link(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;

            Init();
            UpdateBelongings(_sceneObject);

            Awake_Ext();
        }

        public void SetUp(SceneVariablesSO _sceneVariablesSO, SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
            sceneVariablesSO = _sceneVariablesSO;
            UpdateSceneVariables();
        }
        public void OnSceneObjectEnable()
        {
            RegisterElements();

            OnEnable_Ext();
        }
        public void OnSceneObjectDisable()
        {
            UnregisterElements();

            OnDisable_Ext();
        }
        #endregion

        #region Base
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
        protected abstract void UpdateBelongings(SceneObject sceneObject);
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
    }
}
