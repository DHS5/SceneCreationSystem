using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneSpawner : SceneObject
    {
        [Header("Spawner")]
        [SerializeReference, SubclassPicker] public SceneProfile profile;
        [Space(50f)]
        [SerializeReference, SubclassPicker] public List<SceneProfile> profiles;
        
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            profile.SetUp(sceneVariablesSO);
        }
    }
}
