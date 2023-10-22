using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneDependency : SceneState.ISceneVarSetupable
    {
        [SerializeField] private SceneVariablesSO sceneVariablesSO;

        [SerializeField] private SceneVarTween sceneVar;
        [SerializeField] private List<SceneObject> sceneObjects;

        [SerializeField] private float propertyHeight;

        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;
            sceneVar.SetUp(_sceneVariablesSO, SceneVarType.INT, false, true);
        }

        public static List<SceneObject> GetDependencies(BaseVariablesContainer container, int UID)
        {
            List<SceneObject> sceneObjects = new();

            foreach (var so in GameObject.FindObjectsOfType<SceneObject>())
            {
                if ((container is IntersceneVariablesSO || 
                    (container is SceneVariablesSO sceneVariablesSO && so.SceneVariablesSO == sceneVariablesSO)) 
                    && so.DependOn(UID))
                {
                    sceneObjects.Add(so);
                }
            }

            return sceneObjects;
        }
        public static bool IsValidInCurrentScene(BaseVariablesContainer container)
        {
            if (container is IntersceneVariablesSO) return true;

            if (container is SceneVariablesSO sceneVariablesSO)
            {
                SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
                if (manager != null)
                {
                    return manager.SceneVariablesSO == sceneVariablesSO;
                }
            }
            return false;
        }
    }
}
