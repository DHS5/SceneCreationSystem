using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneDependency : SceneState.ISceneVarSetupable
    {
        [SerializeField] private SceneVarTween sceneVar;

        public void SetUp(SceneVariablesSO sceneVariablesSO)
        {
            sceneVar.SetUp(sceneVariablesSO, SceneVarType.BOOL, false, true);
        }
    }
}
