using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CreateAssetMenu(fileName = "SceneObject Layer Database", menuName = "Scene Creation/Database/Layers")]
    public class SceneObjectLayerDatabase : FlagDatabase
    {
        public static SceneObjectLayerDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return SceneCreationSettings.instance.SceneObjectSettings.LayerDatabase;
#else
                if (Application.isPlaying)
                {
                    return SceneManager.Settings.SceneObjectLayerDatabase;
                }

                return null;
#endif
            }
        }
    }
}
