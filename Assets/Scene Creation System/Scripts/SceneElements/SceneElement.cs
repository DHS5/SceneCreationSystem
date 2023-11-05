using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneElement
    {
        public SceneVariablesSO SceneVars => sceneVariablesSO;
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;

        public BaseSceneObject SceneObj => sceneObject;
        [SerializeField] protected BaseSceneObject sceneObject;


        public void Setup(SceneVariablesSO _sceneVariablesSO, BaseSceneObject _sceneObject)
        {
            sceneVariablesSO = _sceneVariablesSO;
            sceneObject = _sceneObject;
        }

        #region Debug
        protected void Debug0(object o)
        {
            sceneObject.DebugThis(0, o);
        }
        protected void Debug1(object o)
        {
            sceneObject.DebugThis(1, o);
        }
        protected void Debug2(object o)
        {
            sceneObject.DebugThis(2, o);
        }
        protected void Debug3(object o)
        {
            sceneObject.DebugThis(3, o);
        }
        protected void Debug4(object o)
        {
            sceneObject.DebugThis(4, o);
        }
        protected void Debug5(object o)
        {
            sceneObject.DebugThis(5, o);
        }
        #endregion
    }
}
